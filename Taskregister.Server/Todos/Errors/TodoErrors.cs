using Taskregister.Server.Shared;

namespace Taskregister.Server.Todos.Errors;

public static class TodoErrors
{
    public static Error NotFoundTodoIdForUserId(int userId, int taskId)
        => new("Todo.NotFoundTodoIdForUserId", $"The task with Id='{taskId}' was not found for user with userId='{userId}'.");
    public static Error CantModifyCompleted(int taskId) => new("Todo.CantModifyCompleted", $"Can't modify completed task with {taskId} id.");
    public static Error CantDeleteCompleted(int taskId) => new("Todo.CantDeleteCompleted", $"Can't delete completed task with {taskId} id.");
    public static Error CantUpdateCompleted(int taskId) => new("Todo.CantUpdateCompleted", $"Can't update completed task with {taskId} id.");
    public static Error CantChangeState(int taskId, Contstants.State from, Contstants.State to) => new("Todo.CantChangeState", $"Unable to change state of task with {taskId} id from {from} status to {to}.");
}
