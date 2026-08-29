using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Abstractions;

/// <summary>
/// This service is used to manage the matches, each match belongs to a sport.
/// </summary>
public interface IMatchService
{
    /// <summary>
    /// GetMatches returns a page with match information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<MatchRecord>>> GetMatches(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetMatch will provide the information about a match given its id.
    /// </summary>
    public Task<ServiceResponse<MatchRecord>> GetMatch(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// AddMatch adds a match and verifies if the requesting user has permissions to add one.
    /// </summary>
    public Task<ServiceResponse> AddMatch(MatchAddRecord match, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdateMatch updates a match and verifies if the requesting user has permissions to update it.
    /// </summary>
    public Task<ServiceResponse> UpdateMatch(MatchUpdateRecord match, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// SetMatchResult records the final score of a match and marks it as finished.
    /// </summary>
    public Task<ServiceResponse> SetMatchResult(Guid id, MatchSetResultRecord result, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeleteMatch deletes a match and verifies if the requesting user has permissions to delete it.
    /// </summary>
    public Task<ServiceResponse> DeleteMatch(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default);
}
