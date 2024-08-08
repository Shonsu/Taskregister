using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Taskregister.Server.Todos.Constants;

namespace Taskregister.Server.Todos.Controller.Dto;

public class CreateTodoDto
{
    [JsonRequired]
    public TodoType Type { get; set; }
    [JsonRequired]
    public Priority Priority { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public List<int> TagIds { get; set; }
}

