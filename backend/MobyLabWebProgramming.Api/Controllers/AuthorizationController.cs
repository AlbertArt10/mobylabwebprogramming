using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Handlers;
using MobyLabWebProgramming.Database.Repository.Enums;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;

/// <summary>
/// This is a controller to respond to authentication requests.
/// Inject the required services through the constructor.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class AuthorizationController(ILogger<AuthorizationController> logger, IUserService userService) : BaseResponseController(logger) // The controller must inherit ControllerBase or its derivations, in this case BaseResponseController.
{
    /// <summary>
    /// This method will respond to login requests.
    /// </summary>
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/Authorization/Login having a JSON body deserialized as a LoginRecord.
    public async Task<ActionResult<RequestResponse<LoginResponseRecord>>> Login([FromBody] LoginRecord login) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        return FromServiceResponse(await userService.Login(login with { Password = PasswordUtils.HashPassword(login.Password)})); // The "with" keyword works only with records and it creates another object instance with the updated properties. 
    }

    /// <summary>
    /// This method will respond to registration requests, it is the only route that creates an account without being authenticated.
    /// Note that the role is set here and not taken from the request, otherwise anyone could register as an administrator.
    /// </summary>
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/Authorization/Register.
    public async Task<ActionResult<RequestResponse<LoginResponseRecord>>> Register([FromBody] RegisterRecord register)
    {
        var hashedPassword = PasswordUtils.HashPassword(register.Password);
        var addUserResponse = await userService.AddUser(new()
        {
            Name = register.Name,
            Email = register.Email,
            Password = hashedPassword,
            Role = UserRoleEnum.User, // A new account always gets the least privileged role.
            FavoriteTeam = register.FavoriteTeam,
            Country = register.Country
        }); // The requesting user is left null, which tells the service that the application itself is adding the user and no permission check is needed.

        return addUserResponse.IsOk ?
            FromServiceResponse(await userService.Login(new(register.Email, hashedPassword))) : // The user is logged in right away so the client gets a token without a second request.
            ErrorMessageResult<LoginResponseRecord>(addUserResponse.Error);
    }
}
