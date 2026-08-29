using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a simple specification to filter the match entities from the database via the constructors.
/// The specification will extract the raw entities from the database without a projection.
/// </summary>
public sealed class MatchSpec : Specification<Match>
{
    public MatchSpec(Guid id) => Query.Where(e => e.Id == id);
}
