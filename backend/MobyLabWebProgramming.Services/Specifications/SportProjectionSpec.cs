using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a specification to filter the sport entities and map them to a SportRecord object via the constructors.
/// The specification will project the entity onto a DTO so it isn't tracked by the framework.
/// </summary>
public sealed class SportProjectionSpec : Specification<Sport, SportRecord>
{
    /// <summary>
    /// In this constructor is the projection/mapping expression used to get a SportRecord object directly from the database.
    /// </summary>
    public SportProjectionSpec(bool orderByCreatedAt = false) =>
        Query.OrderByDescending(e => e.CreatedAt, orderByCreatedAt)
            .Select(e => new()
            {
                Id = e.Id,
                Name = e.Name,
                MatchCount = e.Matches.Count, // The count is translated to a subquery, the matches themselves are never loaded.
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });

    public SportProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id); // This constructor will call the first declared constructor with the default parameter.

    public SportProjectionSpec(string? search) : this(true) // This constructor will call the first declared constructor with 'true' as the parameter.
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Name, searchExpr)); // This is translated to something like "where sport.Name ilike '%str%'" in the database.
    }
}
