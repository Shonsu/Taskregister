using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Taskregister.Server.Task.Contstants;

namespace Taskregister.Server.Persistance
{
    public class TaskRegisterDbContext(DbContextOptions<TaskRegisterDbContext> options) : DbContext(options)
    {
        internal DbSet<User.Entities.User> Users { get; set; }
        internal DbSet<Task.Entities.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Task.Entities.Task>()
                .Property(t => t.State).HasConversion(new EnumToStringConverter<State>());
        }

        // For all Enums in Entities
        // protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        // {
        //     base.ConfigureConventions(configurationBuilder);
        //     configurationBuilder.Properties<Enum>().HaveConversion<string>();
        // }
    }
}