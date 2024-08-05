using Microsoft.AspNetCore.Mvc;
using Taskregister.Server.Shared;
using Taskregister.Server.Task.Controller.Dto;
using Taskregister.Server.Task.Contstants;
using Taskregister.Server.Task.Services;
using Taskregister.Server.Task.Services.Dto;

namespace Taskregister.Server.Task.Controller;

[ApiController]
[Route("api/")]
public class TaskController(ILogger<TaskController> logger, ITaskService taskService) : ControllerBase
{
    [HttpGet("{userEmail}/[controller]")]
    public async Task<ActionResult<IEnumerable<Entities.Task>>> GetAllTasks([FromRoute] string userEmail,
        [FromQuery] QueryParameters query)
    {
        var result = await taskService.GetTasksForUser(userEmail, query);
        return result.Match(onSuccess: Ok, onFailure: NotFound);

        //return result.Match<IEnumerable<Entities.Task>>(onSuccess: result => Ok(result),
        // onFailure: error => NotFound(error));

        //if (result.IsFailure)
        //{
        //    return NotFound(result.Error);
        //}
        //return Ok(result.Value);
    }

    [HttpGet("{userEmail}/[controller]/{taskId}")]
    public async Task<ActionResult<Entities.Task>> GetTaskForUser([FromRoute] string userEmail, [FromRoute] int taskId)
    {
        var result = await taskService.GetTaskForUser(userEmail, taskId);
        
        // return Ok(result.Value);
        return result.Match(onSuccess: Ok, onFailure: BadRequest);
    }

    [HttpPost("{userEmail}/[controller]")]
    public async Task<ActionResult<int>> CreateTask([FromBody] CreateTaskDto createTaskDto,
        [FromRoute] string userEmail)
    {
        var result = await taskService.CreateTaskAsync(createTaskDto, userEmail);
        return result.Match<int>(
            onSuccess: r =>
                CreatedAtAction(nameof(GetTaskForUser), new { userEmail = userEmail, taskId = result }, r),
            onFailure: error => BadRequest(error));

        // return Ok(taskId);
    }

    [HttpPut("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto, [FromRoute] string userEmail,
        [FromRoute] int taskId)
    {
        var result = await taskService.UpdateTaskAsync(updateTaskDto, userEmail, taskId);
        return result.Match(onSuccess: Ok, onFailure: BadRequest);
        //return NoContent();
    }

    [HttpDelete("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> DeleteTask([FromRoute] string userEmail, [FromRoute] int taskId)
    {
        var result = await taskService.DeleteTaskAsync(userEmail, taskId);
        return result.Match(onSuccess: Ok, onFailure: error => BadRequest(error));
        // return NoContent();
    }

    [HttpPatch("{userEmail}/[controller]/{taskId}/endDate")]
    public async Task<IActionResult> ExtendEndDate([FromRoute] string userEmail, [FromRoute] int taskId,
        [FromBody] ExtendBy days)
    {
        var result = await taskService.ExtendEndDate(userEmail, taskId, days);
        //return Ok(id);
        return result.Match(onSuccess: r => Ok(r), onFailure: error => BadRequest(error));
    }

    [HttpPatch("{userEmail}/[controller]/{taskId}/taskState")]
    public async Task<IActionResult> ChangeTaskState([FromRoute] string userEmail, [FromRoute] int taskId,
        [FromQuery] State newState)
    {
        var result = await taskService.ChangeTaskState(userEmail, taskId, newState);
        return result.Match(
            onSuccess: r =>
                CreatedAtAction(nameof(GetTaskForUser), new { userEmail = userEmail, taskId = taskId }, r),
            onFailure: error => BadRequest(error));
    }
}