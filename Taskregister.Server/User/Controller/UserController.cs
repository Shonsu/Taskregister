using Microsoft.AspNetCore.Mvc;
using Taskregister.Server.User.Controllers.Dto;
using Taskregister.Server.User.Repository;

namespace Taskregister.Server.User.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        var userId = await userRepository.CreateUserAsync(new Entities.User() { Email = createUserDto.Email });
        return Ok(userId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entities.User>>> GetAllUsers()
    {
        var users = await userRepository.GetAllUsersAsync();
        return Ok(users);
    }
}