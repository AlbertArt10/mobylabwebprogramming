using MobyLabWebProgramming.Database.Repository.Enums;
using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

/// <summary>
/// This entity stores a match between two teams, the scores are null until the match is finished.
/// </summary>
public class Match : BaseEntity
{
    /// <summary>
    /// This property is used as a foreign key to the sport table in the database and as a correlation key for the ORM.
    /// </summary>
    public Guid SportId { get; set; }

    /// <summary>
    /// This is a navigation property for the ORM to correlate this entity with the entity that it references via the foreign key.
    /// </summary>
    public Sport Sport { get; set; } = null!;
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public DateTime MatchDate { get; set; }
    public MatchStatusEnum Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    /// <summary>
    /// This is a navigation property for the articles written about this match.
    /// </summary>
    public ICollection<Article> Articles { get; set; } = null!;
}
