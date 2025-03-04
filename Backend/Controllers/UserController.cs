using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredUsers([FromQuery] string? gender, [FromQuery] string? email, [FromQuery] string? username)
    {
        var dbUsers = _userService.GetAllUsersWithAddressesAsync().Result;

        bool isSingleResult = _userService.CheckIfIsSingleResult(email, username);

        if(isSingleResult)
        {
            var result = await _userService.FindSingleUser(gender, email, username, dbUsers);
            if(result.IsOk)
            {
                return Ok(result.Data);
            }
            else
            {
                return NotFound(result.ErrorMessage);
            }
        }
        else
        {
            var result = await _userService.FindManyUsers(gender, email, username, dbUsers);
            if(result.IsOk)
            {
                return Ok(result.Data);
            }
            else
            {
                return NotFound(result.ErrorMessage);
            }
        }
    }
}

