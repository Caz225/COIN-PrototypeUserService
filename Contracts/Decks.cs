namespace PrototypeUserService.Contracts;

// Een enkele deck
public sealed record Deck(Guid Id, Guid OwnerId, string Name, List<Guid> CardIds);

// Request om een deck aan te maken
public sealed record CreateDeckRequest(Guid OwnerId, string Name);

// Request om kaarten toe te voegen of te verwijderen
public sealed record ModifyDeckCardsRequest(List<Guid>? AddCardIds, List<Guid>? RemoveCardIds);

// Response bij wijziging
public sealed record DeckResponse(Guid Id, string Name, List<Guid> CardIds);