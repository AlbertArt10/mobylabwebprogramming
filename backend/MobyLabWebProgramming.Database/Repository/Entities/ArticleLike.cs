using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

/// <summary>
/// This entity is the join table between users and articles, it is what makes the Many-To-Many relation possible.
/// A user can like many articles and an article can be liked by many users, but the same pair can appear only once.
/// </summary>
public class ArticleLike : BaseEntity
{
    /// <summary>
    /// This property is used as a foreign key to the user table, it holds the user that gave the like.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// This property is used as a foreign key to the article table, it holds the article that was liked.
    /// </summary>
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
}
