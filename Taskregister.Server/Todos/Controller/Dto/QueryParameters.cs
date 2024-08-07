using Taskregister.Server.Todos.Constants;
using Taskregister.Server.Todos.Contstants;

namespace Taskregister.Server.Todos.Controller.Dto;

public record QueryParameters(Priority? priority, TodoType? todoType, DateTime? from, DateTime? to)
{
}