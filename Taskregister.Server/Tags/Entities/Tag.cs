using Taskregister.Server.Todos.Entities;

namespace Taskregister.Server.Tags.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Todo> Todos { get; set; } = new();
}