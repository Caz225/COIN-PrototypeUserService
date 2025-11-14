using PrototypeUserService.Endpoints;
using PrototypeUserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi(); // .NET 9 ingebouwde OpenAPI (vervangt AddSwaggerGen)
builder.Services.AddHttpClient("Controller", client =>
{
    client.BaseAddress = new Uri("http://localhost:5099"); // controllerpoort later instelbaar
});
builder.Services.AddSingleton<MockUserService>();



var app = builder.Build();

app.MapOpenApi();

// Health endpoint
app.MapGet("/_health", () => Results.Ok(new { status = "ok", service = "UserService" }));

UsersEndpoints.Map(app);
DecksEndpoints.Map(app);

app.Run();
