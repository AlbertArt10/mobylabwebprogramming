using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a specification to filter the match entities and map them to a MatchRecord object via the constructors.
/// The specification will project the entity onto a DTO so it isn't tracked by the framework.
/// </summary>
public sealed class MatchProjectionSpec : Specification<Match, MatchRecord>
{
    /// <summary>
    /// In this constructor is the projection/mapping expression used to get a MatchRecord object directly from the database.
    /// The matches are ordered by their date because for a match the schedule matters more than when the entry was created.
    /// </summary>
    public MatchProjectionSpec(bool orderByMatchDate = false) =>
        Query.OrderBy(e => e.MatchDate, orderByMatchDate)
            .Select(e => new()
            {
                Id = e.Id,
                SportId = e.SportId,
                SportName = e.Sport.Name, // The navigation property is used here, this is translated to a join in the database.
                HomeTeam = e.HomeTeam,
                AwayTeam = e.AwayTeam,
                MatchDate = e.MatchDate,
                Status = e.Status,
                HomeScore = e.HomeScore,
                AwayScore = e.AwayScore,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });

    public MatchProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id); // This constructor will call the first declared constructor with the default parameter.

    public MatchProjectionSpec(string? search) : this(true) // This constructor will call the first declared constructor with 'true' as the parameter.
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.HomeTeam, searchExpr) || // The search looks in both team names and in the sport name.
                         EF.Functions.ILike(e.AwayTeam, searchExpr) ||
                         EF.Functions.ILike(e.Sport.Name, searchExpr));
    }
}
