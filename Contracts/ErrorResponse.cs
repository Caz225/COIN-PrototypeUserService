namespace PrototypeUserService.Contracts;

public sealed record ErrorResponse(bool ok, string code, string message);