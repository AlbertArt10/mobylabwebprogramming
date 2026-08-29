namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to add a match, note that the status and the scores are not here because a new match is always scheduled and without a result.
/// </summary>
public class MatchAddRecord
{
    public Guid SportId { get; set; }
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public DateTime MatchDate { get; set; }
}
