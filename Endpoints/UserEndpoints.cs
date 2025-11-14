using Microsoft.AspNetCore.Http;
using PrototypeUserService.Contracts;
using PrototypeUserService.Services;

namespace PrototypeUserService.Endpoints;

public static class UsersEndpoints
{
    public static void Map(WebApplication app)
    {
        // POST /users/register
        app.MapPost("/users/register", (RegisterRequest req, MockUserService users) =>
        {
            var id = users.Register(req.Username, req.Password);
            return Results.Ok(new { userId = id, req.Username });
        });

        // POST /users/login
        app.MapPost("/users/login", (LoginRequest req, MockUserService users) =>
        {
            var token = users.Login(req.Username, req.Password);
            if (token is null)
                return Results.Json(new ErrorResponse(false, "INVALID_CREDENTIALS", "Gebruiker onbekend of onjuist wachtwoord."),
                    statusCode: StatusCodes.Status401Unauthorized);

            return Results.Ok(new { token });
        });

        // GET /users/{id}
        app.MapGet("/users/{id:guid}", (Guid id, MockUserService users) => 
        {
            var p = users.GetProfile(id); 
            if (p is null)
                return Results.Json(new ErrorResponse(false, "NOT_FOUND", "Gebruiker niet gevonden."),
                    statusCode: StatusCodes.Status404NotFound);

            var (userId, username, wins, losses) = p.Value; 
            return Results.Ok(new UserProfile(userId, username, wins, losses)); 
        });
        
        // PUT /users/{id}
        app.MapPut("/users/{id:guid}", (Guid id, UpdateProfileRequest req, MockUserService users) => 
        {
            var ok = users.UpdateProfile(id, req.Username, req.Wins, req.Losses); 
            if (!ok)
                return Results.Json(new ErrorResponse(false, "NOT_FOUND", "Gebruiker niet gevonden."),
                    statusCode: StatusCodes.Status404NotFound);

            var p = users.GetProfile(id)!.Value; 
            return Results.Ok(new UserProfile(p.Id, p.Username, p.Wins, p.Losses)); 
        });



        // POST /auth/resolve
        app.MapPost("/auth/resolve", (ResolveTokenRequest req, MockUserService users) =>
        {
            var userId = users.ResolveUser(req.Token);
            if (userId is null)
                return Results.Json(new ErrorResponse(false, "INVALID_TOKEN", "Token ongeldig of onbekend."),
                    statusCode: StatusCodes.Status401Unauthorized);

            return Results.Ok(new ResolvedUser(userId.Value));
        });

        // GET /_controller/ping
        app.MapGet("/_controller/ping", async (IHttpClientFactory cf) =>
        {
            var client = cf.CreateClient("Controller");
            try
            {
                using var resp = await client.GetAsync("/");
                return Results.Ok(new { controller = client.BaseAddress!.ToString(), status = (int)resp.StatusCode });
            }
            catch (Exception ex)
            {
                return Results.Ok(new { controller = client.BaseAddress!.ToString(), error = ex.GetType().Name });
            }
        });
    }
}
