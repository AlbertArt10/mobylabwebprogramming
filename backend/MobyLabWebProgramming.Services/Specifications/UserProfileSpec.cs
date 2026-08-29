using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a simple specification to filter the user profile entities from the database via the constructors.
/// The profile is searched by the user it belongs to because a user has at most one profile.
/// </summary>
public sealed class UserProfileSpec : Specification<UserProfile>
{
    public UserProfileSpec(Guid userId) => Query.Where(e => e.UserId == userId);
}
