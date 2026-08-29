using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

/// <summary>
/// This entity groups the matches by the sport they belong to, it is the parent side of a One-To-Many relation.
/// </summary>
public class Sport : BaseEntity
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// This is a navigation property, the matches are fetched only when explicitly requested via an Include query.
    /// </summary>
    public ICollection<Match> Matches { get; set; } = null!;
}
