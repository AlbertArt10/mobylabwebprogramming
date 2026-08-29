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
public class SportService(IRepository<WebAppDatabaseContext> repository) : ISportService
{
    public async Task<ServiceResponse<PagedResponse<SportRecord>>> GetSports(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new SportProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<SportRecord>> GetSport(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new SportProjectionSpec(id), cancellationToken); // Get a sport using a specification on the repository.

        return result != null ?
            ServiceResponse.ForSuccess(result) :
            ServiceResponse.FromError<SportRecord>(SportNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse> AddSport(SportAddRecord sport, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin) // Verify who can add the sport, you can change this however you see fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can add sports!", ErrorCodes.CannotAdd));
        }

        var name = sport.Name.Trim();
        var result = await repository.GetAsync(new SportSpec(name), cancellationToken);

        if (result != null) // Two sports with the same name would only confuse the user, so the name is treated as unique.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The sport already exists!", ErrorCodes.EntityAlreadyExists));
        }

        await repository.AddAsync(new Sport
        {
            Name = name
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateSport(SportUpdateRecord sport, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can update sports!", ErrorCodes.CannotUpdate));
        }

        var entity = await repository.GetAsync(new SportSpec(sport.Id), cancellationToken);

        if (entity == null) // Verify if the sport is not found, you cannot update a non-existing entity.
        {
            return ServiceResponse.FromError(SportNotFound);
        }

        if (!string.IsNullOrWhiteSpace(sport.Name))
        {
            var name = sport.Name.Trim();
            var existingSport = await repository.GetAsync(new SportSpec(name), cancellationToken);

            if (existingSport != null && existingSport.Id != sport.Id) // The name may be taken by another sport, but keeping its own name is allowed.
            {
                return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The sport already exists!", ErrorCodes.EntityAlreadyExists));
            }

            entity.Name = name;
        }

        await repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteSport(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can delete sports!", ErrorCodes.CannotDelete));
        }

        var deletedCount = await repository.DeleteAsync<Sport>(id, cancellationToken); // Delete the entity, the matches of the sport are deleted by the cascade rule.

        return deletedCount > 0 ?
            ServiceResponse.ForSuccess() :
            ServiceResponse.FromError(SportNotFound);
    }

    /// <summary>
    /// This error is used in more than one place, so it is declared once here.
    /// </summary>
    private static ErrorMessage SportNotFound => new(HttpStatusCode.NotFound, "Sport doesn't exist!", ErrorCodes.EntityNotFound);
}
