namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to update a match, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record MatchUpdateRecord(Guid Id, Guid? SportId = null, string? HomeTeam = null, string? AwayTeam = null, DateTime? MatchDate = null);
