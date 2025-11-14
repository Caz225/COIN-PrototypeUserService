namespace PrototypeUserService.Contracts;

public sealed record UserProfile(Guid Id, string Username, int? Wins, int? Losses); // 🟩
public sealed record UpdateProfileRequest(string? Username, int? Wins, int? Losses); // 🟩