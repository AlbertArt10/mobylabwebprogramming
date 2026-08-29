using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a simple specification to filter the sport entities from the database via the constructors.
/// The specification will extract the raw entities from the database without a projection.
/// </summary>
public sealed class SportSpec : Specification<Sport>
{
    public SportSpec(Guid id) => Query.Where(e => e.Id == id);

    public SportSpec(string name) => Query.Where(e => e.Name == name);
}
