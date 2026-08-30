using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a specification to filter the article entities and map them to an ArticleRecord object via the constructors.
/// The specification will project the entity onto a DTO so it isn't tracked by the framework.
/// </summary>
public sealed class ArticleProjectionSpec : Specification<Article, ArticleRecord>
{
    /// <summary>
    /// In this constructor is the projection/mapping expression used to get an ArticleRecord object directly from the database.
    /// </summary>
    public ArticleProjectionSpec(bool orderByCreatedAt = false) =>
        Query.OrderByDescending(e => e.CreatedAt, orderByCreatedAt)
            .Select(e => new()
            {
                Id = e.Id,
                MatchId = e.MatchId,
                MatchName = e.Match.HomeTeam + " - " + e.Match.AwayTeam, // Both navigation properties are used here, they are translated to joins in the database.
                AuthorId = e.AuthorId,
                AuthorName = e.Author.Name,
                Title = e.Title,
                Content = e.Content,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });

    public ArticleProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id); // This constructor will call the first declared constructor with the default parameter.

    /// <summary>
    /// This constructor is used both for the search and for listing the articles of a single match or of a single author.
    /// The filters are optional, so the same specification serves all the read routes of the controller.
    /// </summary>
    public ArticleProjectionSpec(string? search, Guid? matchId = null, Guid? authorId = null) : this(true)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (matchId.HasValue)
        {
            Query.Where(e => e.MatchId == matchId.Value);
        }

        if (authorId.HasValue)
        {
            Query.Where(e => e.AuthorId == authorId.Value);
        }

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Title, searchExpr) || // The search looks in the article itself, in the teams of the match and in the author name.
                         EF.Functions.ILike(e.Content, searchExpr) ||
                         EF.Functions.ILike(e.Match.HomeTeam, searchExpr) ||
                         EF.Functions.ILike(e.Match.AwayTeam, searchExpr) ||
                         EF.Functions.ILike(e.Author.Name, searchExpr));
    }
}
