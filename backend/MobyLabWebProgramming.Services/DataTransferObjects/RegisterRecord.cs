namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to register a new account from the public registration route.
/// Note that it has no role property, the role is decided by the application and not by the client, otherwise anyone could register as an administrator.
/// </summary>
public record RegisterRecord(string Name, string Email, string Password, string? FavoriteTeam = null, string? Country = null);
