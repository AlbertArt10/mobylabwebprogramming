using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Abstractions;

/// <summary>
/// This service is used to manage the likes given by users to articles.
/// Unlike the other services there is no role verification here, any authenticated user may like an article.
/// </summary>
public interface IArticleLikeService
{
    /// <summary>
    /// LikeArticle records the like of the requesting user on an article.
    /// </summary>
    public Task<ServiceResponse> LikeArticle(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// UnlikeArticle removes the like of the requesting user from an article.
    /// </summary>
    public Task<ServiceResponse> UnlikeArticle(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetArticleLikeSummary returns how many likes an article has and whether the requesting user is one of them.
    /// </summary>
    public Task<ServiceResponse<ArticleLikeSummaryRecord>> GetArticleLikeSummary(Guid articleId, UserRecord requestingUser, CancellationToken cancellationToken = default);
}
