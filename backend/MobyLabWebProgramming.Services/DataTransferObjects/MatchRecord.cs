using MobyLabWebProgramming.Database.Repository.Enums;

namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to transfer information about a match to the client application.
/// Note that SportName is taken from the related entity by the projection so the client doesn't need a second request to know the sport.
/// </summary>
public class MatchRecord
{
    public Guid Id { get; set; }
    public Guid SportId { get; set; }
    public string SportName { get; set; } = null!;
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public DateTime MatchDate { get; set; }
    public MatchStatusEnum Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
