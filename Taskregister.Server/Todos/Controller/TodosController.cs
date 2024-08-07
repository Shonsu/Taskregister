using Microsoft.AspNetCore.Mvc;
using Taskregister.Server.Shared;
using Taskregister.Server.Todos.Controller.Dto;
using Taskregister.Server.Todos.Contstants;
using Taskregister.Server.Todos.Entities;
using Taskregister.Server.Todos.Services;

namespace Taskregister.Server.Todos.Controller;

[ApiController]
[Route("api/")]
public class TodosController(ILogger<TodosController> logger, ITodosService todosService) : ControllerBase
{
    [HttpGet("{userEmail}/[controller]")]
    public async Task<ActionResult<IReadOnlyList<Todo>>> GetAllTasks([FromRoute] string userEmail,
        [FromQuery] QueryParameters query)
    {
        var result = await todosService.GetTodosForUser(userEmail, query);
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
    public async Task<ActionResult<Todo>> GetTaskForUser([FromRoute] string userEmail, [FromRoute] int taskId)
    {
        var result = await todosService.GetTodoForUser(userEmail, taskId);
        
        // return Ok(result.Value);
        return result.Match(onSuccess: Ok, onFailure: BadRequest);
    }

    [HttpPost("{userEmail}/[controller]")]
    public async Task<ActionResult<int>> CreateTask([FromBody] CreateTodoDto createTodoDto,
        [FromRoute] string userEmail)
    {
        var result = await todosService.CreateTodoAsync(createTodoDto, userEmail);
        return result.Match(
            onSuccess: r =>
                CreatedAtAction(nameof(GetTaskForUser), new { userEmail, taskId = r }, null),
            onFailure: error => BadRequest(error));

        // return Ok(taskId);
    }

    [HttpPut("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTodoDto updateTodoDto, [FromRoute] string userEmail,
        [FromRoute] int taskId)
    {
        var result = await todosService.UpdateTodoAsync(updateTodoDto, userEmail, taskId);
        return result.Match(onSuccess: Ok, onFailure: BadRequest);
        //return NoContent();
    }

    [HttpDelete("{userEmail}/[controller]/{taskId}")]
    public async Task<IActionResult> DeleteTask([FromRoute] string userEmail, [FromRoute] int taskId)
    {
        var result = await todosService.DeleteTodoAsync(userEmail, taskId);
        return result.Match(onSuccess: Ok, onFailure: error => BadRequest(error));
        // return NoContent();
    }

    [HttpPatch("{userEmail}/[controller]/{taskId}/endDate")]
    public async Task<IActionResult> ExtendEndDate([FromRoute] string userEmail, [FromRoute] int taskId,
        [FromBody] ExtendBy days)
    {
        var result = await todosService.ExtendTodoEndDate(userEmail, taskId, days);
        //return Ok(id);
        return result.Match(onSuccess: r => Ok(r), onFailure: error => BadRequest(error));
    }

    [HttpPatch("{userEmail}/[controller]/{taskId}/taskState")]
    public async Task<IActionResult> ChangeTaskState([FromRoute] string userEmail, [FromRoute] int taskId,
        [FromQuery] State newState)
    {
        var result = await todosService.ChangeTodoState(userEmail, taskId, newState);
        return result.Match(
            onSuccess: r =>
                CreatedAtAction(nameof(GetTaskForUser), new { userEmail, taskId }, r),
            onFailure: error => BadRequest(error));
    }
}