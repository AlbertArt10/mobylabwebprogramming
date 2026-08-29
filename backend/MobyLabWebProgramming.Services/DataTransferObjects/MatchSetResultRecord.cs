namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to record the final score of a match, the match id is taken from the route.
/// </summary>
public record MatchSetResultRecord(int HomeScore, int AwayScore);
