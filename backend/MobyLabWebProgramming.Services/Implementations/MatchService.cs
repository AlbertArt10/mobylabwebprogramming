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
/// Note that the permission verifications are done here and not in the controller, the controller only knows who is asking.
/// </summary>
public class MatchService(IRepository<WebAppDatabaseContext> repository) : IMatchService
{
    public async Task<ServiceResponse<PagedResponse<MatchRecord>>> GetMatches(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new MatchProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<MatchRecord>> GetMatch(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new MatchProjectionSpec(id), cancellationToken); // Get a match using a specification on the repository.

        return result != null ?
            ServiceResponse.ForSuccess(result) :
            ServiceResponse.FromError<MatchRecord>(MatchNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse> AddMatch(MatchAddRecord match, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin) // Verify who can add the match, you can change this however you see fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can add matches!", ErrorCodes.CannotAdd));
        }

        var sport = await repository.GetAsync(new SportSpec(match.SportId), cancellationToken);

        if (sport == null) // A match cannot exist without the sport it belongs to, the foreign key would fail anyway but this gives a clearer error.
        {
            return ServiceResponse.FromError(SportNotFound);
        }

        var validationError = ValidateMatchInput(match.HomeTeam, match.AwayTeam);

        if (validationError != null)
        {
            return ServiceResponse.FromError(validationError);
        }

        await repository.AddAsync(new Match
        {
            SportId = match.SportId,
            HomeTeam = match.HomeTeam.Trim(),
            AwayTeam = match.AwayTeam.Trim(),
            MatchDate = match.MatchDate,
            Status = MatchStatusEnum.Scheduled // A new match is always scheduled, the result is set later.
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateMatch(MatchUpdateRecord match, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can update matches!", ErrorCodes.CannotUpdate));
        }

        var entity = await repository.GetAsync(new MatchSpec(match.Id), cancellationToken);

        if (entity == null) // Verify if the match is not found, you cannot update a non-existing entity.
        {
            return ServiceResponse.FromError(MatchNotFound);
        }

        if (match.SportId.HasValue) // The match can be moved to another sport, but only to one that exists.
        {
            var sport = await repository.GetAsync(new SportSpec(match.SportId.Value), cancellationToken);

            if (sport == null)
            {
                return ServiceResponse.FromError(SportNotFound);
            }

            entity.SportId = match.SportId.Value;
        }

        entity.HomeTeam = !string.IsNullOrWhiteSpace(match.HomeTeam) ? match.HomeTeam.Trim() : entity.HomeTeam;
        entity.AwayTeam = !string.IsNullOrWhiteSpace(match.AwayTeam) ? match.AwayTeam.Trim() : entity.AwayTeam;

        var validationError = ValidateMatchInput(entity.HomeTeam, entity.AwayTeam); // The values are validated after the merge, only one of the teams may have been sent.

        if (validationError != null)
        {
            return ServiceResponse.FromError(validationError);
        }

        entity.MatchDate = match.MatchDate ?? entity.MatchDate;

        await repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> SetMatchResult(Guid id, MatchSetResultRecord result, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can set match results!", ErrorCodes.CannotUpdate));
        }

        if (result.HomeScore < 0 || result.AwayScore < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Scores cannot be negative!", ErrorCodes.InvalidRequest));
        }

        var entity = await repository.GetAsync(new MatchSpec(id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(MatchNotFound);
        }

        entity.HomeScore = result.HomeScore;
        entity.AwayScore = result.AwayScore;
        entity.Status = MatchStatusEnum.Finished; // Recording the score is what finishes a match.

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteMatch(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can delete matches!", ErrorCodes.CannotDelete));
        }

        var deletedCount = await repository.DeleteAsync<Match>(id, cancellationToken); // Delete the entity.

        return deletedCount > 0 ?
            ServiceResponse.ForSuccess() :
            ServiceResponse.FromError(MatchNotFound);
    }

    /// <summary>
    /// The same verifications are needed both when adding and when updating a match, so they are written once here.
    /// </summary>
    private static ErrorMessage? ValidateMatchInput(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam) || string.IsNullOrWhiteSpace(awayTeam))
        {
            return new(HttpStatusCode.BadRequest, "Both teams are required!", ErrorCodes.InvalidRequest);
        }

        return string.Equals(homeTeam.Trim(), awayTeam.Trim(), StringComparison.OrdinalIgnoreCase) ?
            new(HttpStatusCode.BadRequest, "A team cannot play against itself!", ErrorCodes.InvalidRequest) :
            null;
    }

    private static ErrorMessage MatchNotFound => new(HttpStatusCode.NotFound, "Match doesn't exist!", ErrorCodes.EntityNotFound);

    private static ErrorMessage SportNotFound => new(HttpStatusCode.NotFound, "Sport doesn't exist!", ErrorCodes.EntityNotFound);
}
