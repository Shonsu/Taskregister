using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Task.Controller.Dto;

public record QueryParameters(Priority? priority, TaskType? taskType, DateTime? from, DateTime? to)
{
}