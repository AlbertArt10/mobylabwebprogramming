using System.Net;
using MobyLabWebProgramming.Database.Repository;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.DataTransferObjects;
using MobyLabWebProgramming.Services.Specifications;

namespace MobyLabWebProgramming.Services.Implementations;

/// <summary>
/// Inject the required services through the constructor.
/// The like is always given in the name of the user that makes the request, it is never taken from the request itself.
/// </summary>
public class ArticleLikeService(IRepository<WebAppDatabaseContext> repository) : IArticleLikeService
{
    public async Task<ServiceResponse> LikeArticle(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var article = await repository.GetAsync(new ArticleSpec(articleId), cancellationToken);

        if (article == null)
        {
            return ServiceResponse.FromError(ArticleNotFound);
        }

        var existingLike = await repository.GetAsync(new ArticleLikeSpec(articleId, requestingUser.Id), cancellationToken);

        if (existingLike != null) // Liking twice is not an error, the wanted state is already there, so the request simply succeeds.
        {
            return ServiceResponse.ForSuccess(); // Without this check the unique index on the pair of columns would reject the insert.
        }

        await repository.AddAsync(new ArticleLike
        {
            ArticleId = articleId,
            UserId = requestingUser.Id
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UnlikeArticle(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var article = await repository.GetAsync(new ArticleSpec(articleId), cancellationToken);

        if (article == null)
        {
            return ServiceResponse.FromError(ArticleNotFound);
        }

        await repository.DeleteAsync(new ArticleLikeSpec(articleId, requestingUser.Id), cancellationToken); // Removing a like that was never given changes nothing and is not an error either.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<ArticleLikeSummaryRecord>> GetArticleLikeSummary(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var article = await repository.GetAsync(new ArticleSpec(articleId), cancellationToken);

        if (article == null)
        {
            return ServiceResponse.FromError<ArticleLikeSummaryRecord>(ArticleNotFound);
        }

        var likeCount = await repository.GetCountAsync(new ArticleLikeSpec(articleId), cancellationToken); // The count is done in the database, the likes themselves are never loaded.
        var currentUserLike = await repository.GetAsync(new ArticleLikeSpec(articleId, requestingUser.Id), cancellationToken);

        return ServiceResponse.ForSuccess(new ArticleLikeSummaryRecord
        {
            ArticleId = articleId,
            LikeCount = likeCount,
            IsLikedByCurrentUser = currentUserLike != null // This is what lets the client show the button as pressed or not.
        });
    }

    private static ErrorMessage ArticleNotFound => new(HttpStatusCode.NotFound, "Article doesn't exist!", ErrorCodes.EntityNotFound);
}
