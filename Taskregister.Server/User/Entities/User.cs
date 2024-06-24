
namespace Taskregister.Server.User.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public List<Task.Entities.Task> Tasks { get; set; } = [];
}