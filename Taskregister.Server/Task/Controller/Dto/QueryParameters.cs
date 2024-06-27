using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Task.Controller.Dto;

public record QueryParameters(Priority? priority, TaskType? taskType, DateOnly? from, DateOnly? to)
{
}