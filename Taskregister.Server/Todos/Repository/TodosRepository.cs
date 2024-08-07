using Microsoft.EntityFrameworkCore;
using Taskregister.Server.Persistence;
using Taskregister.Server.Todos.Controller.Dto;
using Taskregister.Server.Todos.Entities;

namespace Taskregister.Server.Todos.Repository;

public interface ITodosRepository
{
    Task<int> CreateTodo(Todo todo);

    Task<Todo?> GetTodoByIdAsync(int taskId);

    Task<Todo?> GetTodoByUserIdAndTodoIdAsync(int userId, int taskId);

    Task SaveChangesAsync();

    Task Delete(Todo todo);

    Task<IReadOnlyList<Todo>> GetAllMatchingTodoForUser(User.Entities.User user, QueryParameters parameters);
}

public class TodosRepository(TodosRegisterDbContext dbContext, ILogger<TodosRepository> logger) : ITodosRepository
{
    public async Task<int> CreateTodo(Todo todo)
    {
        await dbContext.Todos.AddAsync(todo);
        await dbContext.SaveChangesAsync();
        return todo.Id;
    }

    public async Task Delete(Todo todo)
    {
        dbContext.Todos.Remove(todo);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Todo>> GetAllMatchingTodoForUser(User.Entities.User user, QueryParameters parameters)
    {
        logger.LogInformation($"Parameters:  {parameters}");
        if (parameters.from != null)
        {
            logger.LogInformation($"Local time -from- :{parameters.from!.Value.ToLocalTime()}");
        }
        if (parameters.to != null)
        {
            logger.LogInformation($"Local time -to- :{parameters.to!.Value.ToLocalTime()}");
        }

        IQueryable<Todo> todos = dbContext.Todos.Where(t => t.UserId == user.Id).AsQueryable();

        todos = todos.Where(t => parameters.priority == null || t.Priority.Equals(parameters.priority));
        todos = todos.Where(t => parameters.todoType == null || t.Type.Equals(parameters.todoType));
        todos = todos.Where(t => parameters.from == null || t.EndDate >= parameters.from!.Value.ToLocalTime());
        todos = todos.Where(t => parameters.to == null || t.EndDate <= parameters.to!.Value.ToLocalTime()); //.ToDateTime(new TimeOnly(0, 0)).AddDays(1)

        return await todos.ToListAsync();
    }

    public async Task<Todo?> GetTodoByIdAsync(int taskId)
    {
        return await dbContext.Todos.FindAsync(taskId);
    }

    public async Task<Todo?> GetTodoByUserIdAndTodoIdAsync(int userId, int taskId)
    {
        return await dbContext.Todos.Where(t => t.Id == taskId && t.UserId == userId).SingleOrDefaultAsync();
    }

    public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
}