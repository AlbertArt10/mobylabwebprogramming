using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.Authorization;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;

/// <summary>
/// This is the controller for CRUD operations on matches.
/// Besides the basic operations it also has SetResult, which is the action that finishes a match.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class MatchController(ILogger<MatchController> logger, IUserService userService, IMatchService matchService) : AuthorizedController(logger, userService)
{
    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a page of matches.
    /// </summary>
    [HttpGet] // This attribute will make the controller respond to a HTTP GET request on the route /api/Match/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<MatchRecord>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        return FromServiceResponse(await matchService.GetMatches(pagination));
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a match.
    /// </summary>
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/Match/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<MatchRecord>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        return FromServiceResponse(await matchService.GetMatch(id));
    }

    /// <summary>
    /// This method implements the Create operation (C from CRUD) of a match.
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/Match/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] MatchAddRecord match) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        var currentUser = await GetCurrentUser(); // The service needs to know who is asking to verify the permissions.

        return currentUser.Result != null ?
            FromServiceResponse(await matchService.AddMatch(match, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Update operation (U from CRUD) on a match.
    /// </summary>
    [Authorize]
    [HttpPut] // This attribute will make the controller respond to a HTTP PUT request on the route /api/Match/Update.
    public async Task<ActionResult<RequestResponse>> Update([FromBody] MatchUpdateRecord match)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await matchService.UpdateMatch(match, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method records the final score of a match, the id is taken from the route and the scores from the body.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}")] // This attribute will make the controller respond to a HTTP PUT request on the route /api/Match/SetResult/<some_guid>.
    public async Task<ActionResult<RequestResponse>> SetResult([FromRoute] Guid id, [FromBody] MatchSetResultRecord result)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await matchService.SetMatchResult(id, result, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Delete operation (D from CRUD) on a match.
    /// Note that in the HTTP RFC you cannot have a body for DELETE operations.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}")] // This attribute will make the controller respond to an HTTP DELETE request on the route /api/Match/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await matchService.DeleteMatch(id, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }
}
