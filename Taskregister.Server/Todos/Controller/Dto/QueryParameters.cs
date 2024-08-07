using Taskregister.Server.Todos.Contstants;

namespace Taskregister.Server.Todos.Controller.Dto;

public record QueryParameters(Priority? priority, TaskType? taskType, DateTime? from, DateTime? to)
{
}