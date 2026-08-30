using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.Authorization;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;

/// <summary>
/// This is the controller for the likes given to articles.
/// All the routes need a JWT because a like belongs to the user that gives it, but no particular role is required.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class ArticleLikeController(ILogger<ArticleLikeController> logger, IUserService userService, IArticleLikeService articleLikeService) : AuthorizedController(logger, userService)
{
    /// <summary>
    /// This method records the like of the current user on an article.
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpPost("{articleId:guid}")] // This attribute will make the controller respond to a HTTP POST request on the route /api/ArticleLike/Like/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Like([FromRoute] Guid articleId) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        var currentUser = await GetCurrentUser(); // The service needs to know who is asking because the like is recorded in their name.

        return currentUser.Result != null ?
            FromServiceResponse(await articleLikeService.LikeArticle(articleId, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method removes the like of the current user from an article.
    /// </summary>
    [Authorize]
    [HttpDelete("{articleId:guid}")] // This attribute will make the controller respond to an HTTP DELETE request on the route /api/ArticleLike/Unlike/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Unlike([FromRoute] Guid articleId)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await articleLikeService.UnlikeArticle(articleId, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method returns how many likes an article has and whether the current user is one of them.
    /// </summary>
    [Authorize]
    [HttpGet("{articleId:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/ArticleLike/GetSummary/<some_guid>.
    public async Task<ActionResult<RequestResponse<ArticleLikeSummaryRecord>>> GetSummary([FromRoute] Guid articleId)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await articleLikeService.GetArticleLikeSummary(articleId, currentUser.Result)) :
            ErrorMessageResult<ArticleLikeSummaryRecord>(currentUser.Error);
    }
}
