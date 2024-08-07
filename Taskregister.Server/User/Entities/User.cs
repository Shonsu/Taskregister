using Taskregister.Server.Todos.Entities;

namespace Taskregister.Server.User.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public List<Todo> Tasks { get; set; } = [];
}