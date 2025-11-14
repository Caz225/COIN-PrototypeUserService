namespace PrototypeUserService.Contracts;

// Kaartinformatie (mock)
public sealed record Card(Guid Id, string Name);

// Overzicht van kaarten per gebruiker
public sealed record OwnedCardsResponse(Guid UserId, List<Card> Cards);