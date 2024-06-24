using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Task.Services.Dto;

public class CreateTaskDto
{
    [JsonRequired]
    public TaskType Type { get; set; }
    [JsonRequired]
    public Priority Priority { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
}

