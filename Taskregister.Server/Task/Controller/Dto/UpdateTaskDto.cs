using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Task.Controller.Dto
{
    public class UpdateTaskDto
    {
        public TaskType Type { get; set; }
        public Priority Priority { get; set; }
        public State State { get; set; }
        public string? Description { get; set; }
    }
}