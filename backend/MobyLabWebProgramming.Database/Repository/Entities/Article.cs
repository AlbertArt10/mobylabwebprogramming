using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

/// <summary>
/// This entity stores an article written about a match, it is the child side of two One-To-Many relations at once.
/// An article belongs to one match and it is written by one user, while a match and a user can have many articles.
/// </summary>
public class Article : BaseEntity
{
    /// <summary>
    /// This property is used as a foreign key to the match table in the database and as a correlation key for the ORM.
    /// </summary>
    public Guid MatchId { get; set; }

    /// <summary>
    /// This is a navigation property for the ORM to correlate this entity with the entity that it references via the foreign key.
    /// </summary>
    public Match Match { get; set; } = null!;

    /// <summary>
    /// This property is used as a foreign key to the user table, it holds the user that wrote the article.
    /// </summary>
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}
