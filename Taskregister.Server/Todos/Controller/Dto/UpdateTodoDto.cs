using Taskregister.Server.Todos.Constants;

namespace Taskregister.Server.Todos.Controller.Dto
{
    public class UpdateTodoDto
    {
        public TodoType Type { get; set; }
        public Priority Priority { get; set; }
        public string? Description { get; set; }
        public List<int> TagIds { get; set; }
    }
}