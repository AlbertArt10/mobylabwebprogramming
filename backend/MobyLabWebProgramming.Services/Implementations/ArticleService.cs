using System.Net;
using MobyLabWebProgramming.Database.Repository;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.DataTransferObjects;
using MobyLabWebProgramming.Services.Specifications;

namespace MobyLabWebProgramming.Services.Implementations;

/// <summary>
/// Inject the required services through the constructor.
/// The permissions here are on two levels, the role decides who may write at all while the author of the entry decides who may change it.
/// </summary>
public class ArticleService(IRepository<WebAppDatabaseContext> repository) : IArticleService
{
    public async Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticles(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new ArticleProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<ArticleRecord>> GetArticle(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new ArticleProjectionSpec(id), cancellationToken); // Get an article using a specification on the repository.

        return result != null ?
            ServiceResponse.ForSuccess(result) :
            ServiceResponse.FromError<ArticleRecord>(ArticleNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticlesByMatch(Guid matchId, PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var match = await repository.GetAsync(new MatchSpec(matchId), cancellationToken);

        if (match == null) // An empty page and a page for a match that doesn't exist are different situations, so they get different answers.
        {
            return ServiceResponse.FromError<PagedResponse<ArticleRecord>>(MatchNotFound);
        }

        var result = await repository.PageAsync(pagination, new ArticleProjectionSpec(pagination.Search, matchId), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticlesByAuthor(Guid authorId, PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetAsync(new UserSpec(authorId), cancellationToken);

        if (author == null)
        {
            return ServiceResponse.FromError<PagedResponse<ArticleRecord>>(CommonErrors.UserNotFound);
        }

        var result = await repository.PageAsync(pagination, new ArticleProjectionSpec(pagination.Search, authorId: authorId), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddArticle(ArticleAddRecord article, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (!CanWriteArticles(requestingUser)) // Here the role decides, an ordinary user cannot write articles at all.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only analysts or admins can add articles!", ErrorCodes.CannotAdd));
        }

        var match = await repository.GetAsync(new MatchSpec(article.MatchId), cancellationToken);

        if (match == null) // An article cannot exist without the match it is written about.
        {
            return ServiceResponse.FromError(MatchNotFound);
        }

        var validationError = ValidateArticleInput(article.Title, article.Content);

        if (validationError != null)
        {
            return ServiceResponse.FromError(validationError);
        }

        await repository.AddAsync(new Article
        {
            MatchId = article.MatchId,
            AuthorId = requestingUser.Id, // The author is the user that makes the request, it is never taken from the body.
            Title = article.Title.Trim(),
            Content = article.Content.Trim()
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateArticle(ArticleUpdateRecord article, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(new ArticleSpec(article.Id), cancellationToken); // The entity is needed before the verification, the permission depends on who wrote it.

        if (entity == null) // Verify if the article is not found, you cannot update a non-existing entity.
        {
            return ServiceResponse.FromError(ArticleNotFound);
        }

        if (!CanEditArticle(requestingUser, entity)) // Here the author decides, an analyst cannot change the article of another analyst.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the author or the admin can update this article!", ErrorCodes.CannotUpdate));
        }

        if (article.MatchId.HasValue) // The article can be moved to another match, but only to one that exists.
        {
            var match = await repository.GetAsync(new MatchSpec(article.MatchId.Value), cancellationToken);

            if (match == null)
            {
                return ServiceResponse.FromError(MatchNotFound);
            }

            entity.MatchId = article.MatchId.Value;
        }

        var validationError = ValidateArticleUpdate(article.Title, article.Content);

        if (validationError != null)
        {
            return ServiceResponse.FromError(validationError);
        }

        entity.Title = article.Title != null ? article.Title.Trim() : entity.Title;
        entity.Content = article.Content != null ? article.Content.Trim() : entity.Content;

        await repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteArticle(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(new ArticleSpec(id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(ArticleNotFound);
        }

        if (!CanEditArticle(requestingUser, entity))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the author or the admin can delete this article!", ErrorCodes.CannotDelete));
        }

        await repository.DeleteAsync<Article>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }

    /// <summary>
    /// Writing articles is a matter of role, both analysts and administrators are allowed to.
    /// </summary>
    private static bool CanWriteArticles(UserRecord user) => user.Role is UserRoleEnum.Admin or UserRoleEnum.Analyst;

    /// <summary>
    /// Changing an existing article is a matter of ownership, the administrator is the only one that can touch what others wrote.
    /// </summary>
    private static bool CanEditArticle(UserRecord user, Article article) => user.Role == UserRoleEnum.Admin || article.AuthorId == user.Id;

    private static ErrorMessage? ValidateArticleInput(string title, string content)
    {
        return string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) ?
            new ErrorMessage(HttpStatusCode.BadRequest, "Title and content are required!", ErrorCodes.InvalidRequest) :
            null;
    }

    /// <summary>
    /// On an update a missing value means "leave it as it is", but a value made only of spaces is a mistake and is rejected.
    /// </summary>
    private static ErrorMessage? ValidateArticleUpdate(string? title, string? content)
    {
        if (title != null && string.IsNullOrWhiteSpace(title))
        {
            return new(HttpStatusCode.BadRequest, "Title cannot be empty!", ErrorCodes.InvalidRequest);
        }

        return content != null && string.IsNullOrWhiteSpace(content) ?
            new ErrorMessage(HttpStatusCode.BadRequest, "Content cannot be empty!", ErrorCodes.InvalidRequest) :
            null;
    }

    private static ErrorMessage ArticleNotFound => new(HttpStatusCode.NotFound, "Article doesn't exist!", ErrorCodes.EntityNotFound);

    private static ErrorMessage MatchNotFound => new(HttpStatusCode.NotFound, "Match doesn't exist!", ErrorCodes.EntityNotFound);
}
