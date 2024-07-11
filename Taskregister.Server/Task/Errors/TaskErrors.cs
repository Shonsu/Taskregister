using Taskregister.Server.Shared;

namespace Taskregister.Server.Task.Errors
{
    public static class TaskErrors
    {
        public static Error NotFoundByUserEmailAndTaskId(string userEmail, int taskId)
            => new("Tasks.NotFoundByUserEmailAndTaskId", $"The task with Id='{taskId}' was not found for user with Email='{userEmail}'.");
        public static Error CantModifyCompleted(int taskId) => new("Tasks.CantModifyCompleted", $"Can't modify completed task with {taskId} id.");
        public static Error CantDeleteCompleted(int taskId) => new("Tasks.CantDeleteCompleted", $"Can't delete completed task with {taskId} id.");
        public static Error CantUpdateCompleted(int taskId) => new("Tasks.CantUpdateCompleted", $"Can't update completed task with {taskId} id.");
        public static Error CantChangeState(int taskId, Contstants.State from, Contstants.State to) => new("Tasks.CantChangeState", $"Unable to change state of task with {taskId} id from {from} status to {to}.");
    }
}
