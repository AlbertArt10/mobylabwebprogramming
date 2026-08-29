using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

/// <summary>
/// This entity stores the additional profile details of a user, it is an example for a One-To-One relation.
/// The details are kept in their own table so that the user table holds only what the authentication needs.
/// </summary>
public class UserProfile : BaseEntity
{
    /// <summary>
    /// This property is used as a foreign key to the user table in the database and as a correlation key for the ORM.
    /// Because the relation is One-To-One this column also has a unique index, otherwise a user could have more than one profile.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// This is a navigation property for the ORM to correlate this entity with the entity that it references via the foreign key.
    /// </summary>
    public User User { get; set; } = null!;
    public string? FavoriteTeam { get; set; }
    public string? Country { get; set; }
}
