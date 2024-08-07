using Microsoft.EntityFrameworkCore;
using Taskregister.Server.Persistance;
using Taskregister.Server.Task.Controller.Dto;

namespace Taskregister.Server.Task.Repository;

public interface ITaskRepository
{
    Task<int> CreateTask(Entities.Task task);

    Task<Entities.Task?> GetTaskByIdAsync(int taskId);

    Task<Entities.Task?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId);

    System.Threading.Tasks.Task SaveChangesAsync();

    System.Threading.Tasks.Task Delete(Task.Entities.Task task);

    Task<IReadOnlyList<Entities.Task>> GetAllMatchingTaskForUser(User.Entities.User user, QueryParameters parameters);
}

public class TaskRepository(TaskRegisterDbContext dbContext, ILogger<TaskRepository> logger) : ITaskRepository
{
    public async Task<int> CreateTask(Entities.Task task)
    {
        await dbContext.Tasks.AddAsync(task);
        await dbContext.SaveChangesAsync();
        return task.Id;
    }

    public async System.Threading.Tasks.Task Delete(Entities.Task task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Entities.Task>> GetAllMatchingTaskForUser(User.Entities.User user, QueryParameters parameters)
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

        IQueryable<Entities.Task> tasks = dbContext.Tasks.Where(t => t.UserId == user.Id).AsQueryable();

        tasks = tasks.Where(t => parameters.priority == null || t.Priority.Equals(parameters.priority));
        tasks = tasks.Where(t => parameters.taskType == null || t.Type.Equals(parameters.taskType));
        tasks = tasks.Where(t => parameters.from == null || t.EndDate >= parameters.from!.Value.ToLocalTime());
        tasks = tasks.Where(t => parameters.to == null || t.EndDate <= parameters.to!.Value.ToLocalTime()); //.ToDateTime(new TimeOnly(0, 0)).AddDays(1)

        return await tasks.ToListAsync();
    }

    public async Task<Entities.Task?> GetTaskByIdAsync(int taskId)
    {
        return await dbContext.Tasks.FindAsync(taskId);
    }

    public async Task<Entities.Task?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId)
    {
        return await dbContext.Tasks.Where(t => t.Id == taskId && t.UserId == userId).SingleOrDefaultAsync();
    }

    public async System.Threading.Tasks.Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
}