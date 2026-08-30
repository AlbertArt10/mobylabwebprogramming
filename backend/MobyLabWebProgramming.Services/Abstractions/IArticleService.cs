using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Abstractions;

/// <summary>
/// This service is used to manage the articles written about matches.
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// GetArticles returns a page with article information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticles(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetArticle will provide the information about an article given its id.
    /// </summary>
    public Task<ServiceResponse<ArticleRecord>> GetArticle(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetArticlesByMatch returns only the articles written about one match.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticlesByMatch(Guid matchId, PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetArticlesByAuthor returns only the articles written by one user.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<ArticleRecord>>> GetArticlesByAuthor(Guid authorId, PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// AddArticle adds an article and verifies if the requesting user is allowed to write articles.
    /// </summary>
    public Task<ServiceResponse> AddArticle(ArticleAddRecord article, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdateArticle updates an article and verifies if the requesting user is its author or an administrator.
    /// </summary>
    public Task<ServiceResponse> UpdateArticle(ArticleUpdateRecord article, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeleteArticle deletes an article and verifies if the requesting user is its author or an administrator.
    /// </summary>
    public Task<ServiceResponse> DeleteArticle(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default);
}
