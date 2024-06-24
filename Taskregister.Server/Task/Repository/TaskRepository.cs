using Taskregister.Server.Persistance;

namespace Taskregister.Server.Task.Repository
{
    public interface ITaskRepository
    {
        Task<int> CreateTask(Entities.Task task);
        Task<Entities.Task?> GetTaskByIdAsync(int taskId);
        //Task<Entities.Task?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId);
        System.Threading.Tasks.Task SaveChangesAsync();
        System.Threading.Tasks.Task Delete(Task.Entities.Task task);
    }

    public class TaskRepository(TaskRegisterDbContext dbContext) : ITaskRepository
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

        public async Task<Entities.Task?> GetTaskByIdAsync(int taskId)
        {
            return await dbContext.Tasks.FindAsync(taskId);
        }

        //public Task<Entities.Task?> GetTaskByUserIdAndTaskIdAsync(int userId, int taskId)
        //{
        //    throw new NotImplementedException();
        //}

        public async System.Threading.Tasks.Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
    }
}
