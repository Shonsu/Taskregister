using Microsoft.AspNetCore.Mvc;
using Taskregister.Server.Task.Services.Dto;
using Taskregister.Server.Task.Services;
using Taskregister.Server.Task.Controller.Dto;
using Taskregister.Server.User.Repository;
using Taskregister.Server.Exceptions;

namespace Taskregister.Server.Task.Controller;

[ApiController]
[Route("api/")]
public class TaskController(ITaskService taskService, IUserRepository userRepository) : ControllerBase
{

    [HttpGet("{userEmail}/[controller]")]
    public async Task<ActionResult<IEnumerable<Entities.Task>>> GetAllTasks([FromRoute] string userEmail)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user == null)
        {
            throw new NotFoundException(nameof(Server.User.Entities.User), userEmail);
        }
        return Ok(user.Tasks);
    }
    [HttpPost("{userEmail}/[controller]")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto, [FromRoute] string userEmail)
    {
        int taskId = await taskService.CreateTaskAsync(createTaskDto, userEmail);
        return Ok(taskId);
    }

    [HttpPut("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto, [FromRoute] string userEmail, [FromRoute] int taskId)
    {
        await taskService.UpdateTaskAsync(updateTaskDto, userEmail, taskId);
        return NoContent();
    }

    [HttpDelete("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> DeleteTask([FromRoute] string userEmail, [FromRoute] int taskId)
    {
        await taskService.DeleteTaskAsync(userEmail, taskId);
        return NoContent();
    }

    [HttpPatch("{userEmail}/[controller]/{taskId}/enddate")]
    public async Task<IActionResult> DeleteTask([FromRoute] string userEmail, [FromRoute] int taskId, [FromBody] ExtendBy days)
    {
        int id = await taskService.ExtendEndDate(userEmail, taskId, days);
        return Ok(id);
    }

}
