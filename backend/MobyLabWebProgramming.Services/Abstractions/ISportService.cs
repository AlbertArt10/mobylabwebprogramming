using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Abstractions;

/// <summary>
/// This service is used to manage the sports, a sport groups the matches that belong to it.
/// </summary>
public interface ISportService
{
    /// <summary>
    /// GetSports returns a page with sport information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<SportRecord>>> GetSports(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetSport will provide the information about a sport given its id.
    /// </summary>
    public Task<ServiceResponse<SportRecord>> GetSport(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// AddSport adds a sport and verifies if the requesting user has permissions to add one.
    /// </summary>
    public Task<ServiceResponse> AddSport(SportAddRecord sport, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdateSport updates a sport and verifies if the requesting user has permissions to update it.
    /// </summary>
    public Task<ServiceResponse> UpdateSport(SportUpdateRecord sport, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeleteSport deletes a sport and verifies if the requesting user has permissions to delete it.
    /// </summary>
    public Task<ServiceResponse> DeleteSport(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default);
}
