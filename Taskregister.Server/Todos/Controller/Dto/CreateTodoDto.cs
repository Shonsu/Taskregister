using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Taskregister.Server.Todos.Contstants;

namespace Taskregister.Server.Todos.Services.Dto;

public class CreateTodoDto
{
    [JsonRequired]
    public TaskType Type { get; set; }
    [JsonRequired]
    public Priority Priority { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
}

