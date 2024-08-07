using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Taskregister.Server.Todos.Contstants;
using Taskregister.Server.Todos.Entities;

namespace Taskregister.Server.Persistence
{
    public class TodosRegisterDbContext(DbContextOptions<TodosRegisterDbContext> options) : DbContext(options)
    {
        internal DbSet<User.Entities.User> Users { get; set; }
        internal DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Todo>()
                .Property(t => t.State).HasConversion(new EnumToStringConverter<State>());
           // modelBuilder.Entity<Todo>().ToTable("Todos").HasKey(k => k.Id);
        }

        // For all Enums in Entities to be stored as strings
        // protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        // {
        //     base.ConfigureConventions(configurationBuilder);
        //     configurationBuilder.Properties<Enum>().HaveConversion<string>();
        // }
    }
}