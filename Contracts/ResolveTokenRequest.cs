namespace PrototypeUserService.Contracts;

public sealed record ResolveTokenRequest(string Token);
public sealed record ResolvedUser(Guid UserId);