using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Task.Entities;

public class Task
{
    public int Id { get; set; }
    public TaskType Type { get; set; }
    public Priority Priority { get; set; }
    public DateTime EndDate{ get; set; }
    public State State { get; set; }
    public DateTime DateState { get; set; }
    public string? Description { get; set; }
    public string? ChangeStateRationale { get; set; }
    public int UserId  { get; set; }

}