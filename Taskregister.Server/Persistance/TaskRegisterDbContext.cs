using Microsoft.EntityFrameworkCore;

namespace Taskregister.Server.Persistance
{
    public class TaskRegisterDbContext(DbContextOptions<TaskRegisterDbContext> options) : DbContext(options)
    {
        internal DbSet<User.Entities.User> Users { get; set; }
        internal DbSet<Task.Entities.Task> Tasks { get; set; }
    }
}
