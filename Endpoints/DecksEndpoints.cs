using Microsoft.AspNetCore.Http;
using PrototypeUserService.Contracts;
using PrototypeUserService.Services;

namespace PrototypeUserService.Endpoints;

public static class DecksEndpoints
{
    public static void Map(WebApplication app)
    {
        // ------------------------------
        // GET /cards?ownerId={id}
        // ------------------------------
        app.MapGet("/cards", (Guid ownerId, MockUserService users) =>
        {
            var cards = users.GetOwnedCards(ownerId);
            return Results.Ok(new OwnedCardsResponse(ownerId, cards));
        });

        // ------------------------------
        // POST /cards (mock card toevoegen)
        // ------------------------------
        app.MapPost("/cards", (Guid ownerId, string name, MockUserService users) =>
        {
            users.AddOwnedCard(ownerId, name);
            return Results.Ok(new { message = $"Card '{name}' toegevoegd aan user {ownerId}" });
        });

        // ------------------------------
        // GET /decks?ownerId={id}
        // ------------------------------
        app.MapGet("/decks", (Guid ownerId, MockUserService users) =>
        {
            var decks = users.GetDecks(ownerId);
            return Results.Ok(decks);
        });

        // ------------------------------
        // POST /decks
        // ------------------------------
        app.MapPost("/decks", (CreateDeckRequest req, MockUserService users) =>
        {
            var deck = users.CreateDeck(req.OwnerId, req.Name);
            return Results.Ok(new DeckResponse(deck.Id, deck.Name, deck.CardIds));
        });

        // ------------------------------
        // POST /decks/{deckId}/cards
        // ------------------------------
        app.MapPost("/decks/{deckId:guid}/cards", (Guid deckId, ModifyDeckCardsRequest req, Guid ownerId, MockUserService users) =>
        {
            var deck = users.ModifyDeckCards(ownerId, deckId, req.AddCardIds, req.RemoveCardIds);
            if (deck is null)
                return Results.Json(new ErrorResponse(false, "NOT_FOUND", "Deck niet gevonden."),
                    statusCode: StatusCodes.Status404NotFound);

            return Results.Ok(new DeckResponse(deck.Id, deck.Name, deck.CardIds));
        });
    }
}
