using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.Authorization;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;

/// <summary>
/// This is the controller for CRUD operations on articles.
/// Besides the basic operations it also has GetByMatch and GetByAuthor, which are the two ways an article is usually looked for.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class ArticleController(ILogger<ArticleController> logger, IUserService userService, IArticleService articleService) : AuthorizedController(logger, userService)
{
    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a page of articles.
    /// </summary>
    [HttpGet] // This attribute will make the controller respond to a HTTP GET request on the route /api/Article/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<ArticleRecord>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        return FromServiceResponse(await articleService.GetArticles(pagination));
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on an article.
    /// </summary>
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/Article/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<ArticleRecord>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        return FromServiceResponse(await articleService.GetArticle(id));
    }

    /// <summary>
    /// This method returns only the articles written about one match.
    /// </summary>
    [HttpGet("{matchId:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/Article/GetByMatch/<some_guid>.
    public async Task<ActionResult<RequestResponse<PagedResponse<ArticleRecord>>>> GetByMatch([FromRoute] Guid matchId, [FromQuery] PaginationSearchQueryParams pagination)
    {
        return FromServiceResponse(await articleService.GetArticlesByMatch(matchId, pagination));
    }

    /// <summary>
    /// This method returns only the articles written by one user.
    /// </summary>
    [HttpGet("{authorId:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/Article/GetByAuthor/<some_guid>.
    public async Task<ActionResult<RequestResponse<PagedResponse<ArticleRecord>>>> GetByAuthor([FromRoute] Guid authorId, [FromQuery] PaginationSearchQueryParams pagination)
    {
        return FromServiceResponse(await articleService.GetArticlesByAuthor(authorId, pagination));
    }

    /// <summary>
    /// This method implements the Create operation (C from CRUD) of an article.
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/Article/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] ArticleAddRecord article) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        var currentUser = await GetCurrentUser(); // The service needs to know who is asking, both to verify the permissions and to set the author.

        return currentUser.Result != null ?
            FromServiceResponse(await articleService.AddArticle(article, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Update operation (U from CRUD) on an article.
    /// </summary>
    [Authorize]
    [HttpPut] // This attribute will make the controller respond to a HTTP PUT request on the route /api/Article/Update.
    public async Task<ActionResult<RequestResponse>> Update([FromBody] ArticleUpdateRecord article)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await articleService.UpdateArticle(article, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Delete operation (D from CRUD) on an article.
    /// Note that in the HTTP RFC you cannot have a body for DELETE operations.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}")] // This attribute will make the controller respond to an HTTP DELETE request on the route /api/Article/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await articleService.DeleteArticle(id, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }
}
