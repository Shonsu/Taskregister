using Taskregister.Server.Exceptions;
using Taskregister.Server.Task.Controller.Dto;
using Taskregister.Server.Task.Contstants;
using Taskregister.Server.Task.Repository;
using Taskregister.Server.Task.Services.Dto;
using Taskregister.Server.User.Repository;

namespace Taskregister.Server.Task.Services;

public interface ITaskService
{
    //Task<int> ChangeTaskStatus(string userEmail, int taskId, State state);
    Task<int> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail);
    System.Threading.Tasks.Task DeleteTaskAsync(string userEmail, int taskId);
    Task<int> ExtendEndDate(string userEmail, int taskId, ExtendBy days);
    System.Threading.Tasks.Task UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId);
    // Task<Entities.Task> GetTaskById(int taskId);
}

public class TaskService(IUserRepository userRepository, ITaskRepository taskRepository) : ITaskService
{
    //public async Task<int> ChangeTaskStatus(string userEmail, int taskId, State state)
    //{
    //    var user = await userRepository.GetUserAsync(userEmail);
    //    if (user is null)
    //    {
    //        throw new NotFoundException(nameof(User.Entities.User), userEmail);
    //    }
    //    var task = user.Tasks.SingleOrDefault(t => t.Id == taskId);

    //    if (task is null)
    //    {
    //        throw new NotSupportedException($"User with {userEmail} doesn't have task with {taskId} id.");
    //    }
    //    if (task.State is not State.COMPLETED)
    //    {
    //        task.State = state;
    //    }
    //    return task.Id;
    //}

    public async Task<int> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail)
    {
        var user = await userRepository.GetUserAsync(userEmail);

        if (user is null)
        {
            throw new NotFoundException(nameof(User.Entities.User), userEmail);
        }
        var endDate = createTaskDto.Type switch
        {
            TaskType.TYPE_1 => DateTime.Now.AddDays((double)TaskType.TYPE_1),
            TaskType.TYPE_2 => DateTime.Now.AddDays((double)TaskType.TYPE_2),
            TaskType.TYPE_3 => DateTime.Now.AddDays((double)TaskType.TYPE_3),
            _ => throw new ArgumentOutOfRangeException(nameof(createTaskDto.Type), $"Not expected task type value: {createTaskDto.Type}"),
        };

        var task = new Entities.Task()
        {
            Type = createTaskDto.Type,
            Priority = createTaskDto.Priority,
            EndDate = endDate,
            State = State.NEW,
            DateState = DateTime.Now,
            Description = createTaskDto.Description
        };

        user.Tasks.Add(task);
        await userRepository.SaveChangesAsync();
        return task.Id;
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(string userEmail, int taskId)
    {
        Entities.Task? task = await GetTaskByUserEmailAndTaskId(userRepository, userEmail, taskId);

        if (task!.State is not State.COMPLETED)
        {
            await taskRepository.Delete(task);
        }
        else
        {
            throw new NotSupportedException($"Unable to delete task {taskId} id with status: {task.State}.");
        }
    }

    public async Task<int> ExtendEndDate(string userEmail, int taskId, ExtendBy days)
    {
        Entities.Task? task = await GetTaskByUserEmailAndTaskId(userRepository, userEmail, taskId);
        if (task!.State == State.COMPLETED)
        {
            throw new NotSupportedException($"Can't modify comleted task with {taskId} id.");
        }
        task!.EndDate = task!.EndDate.AddDays(days.days);
        task.ChangeStateRationale = days.extendByDayRationale;
        await taskRepository.SaveChangesAsync();
        return task.Id;
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId)
    {
        Entities.Task? task = await GetTaskByUserEmailAndTaskId(userRepository, userEmail, taskId);

        if (task!.State == State.COMPLETED)
        {
            throw new NotSupportedException($"Can't update comleted task with {taskId} id.");
        }

        if (task.State is not State.COMPLETED && updateTaskDto.State is not State.RESUMED)
        {
            task.State = updateTaskDto.State;
            task.DateState = DateTime.Now;
        }

        task.Type = updateTaskDto.Type;
        task.Priority = updateTaskDto.Priority;
        await taskRepository.SaveChangesAsync();

    }
    private async Task<Entities.Task?> GetTaskByUserEmailAndTaskId(IUserRepository userRepository, string userEmail, int taskId)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            throw new NotFoundException(nameof(User.Entities.User), userEmail);
        }
        var task = user.Tasks.SingleOrDefault(t => t.Id == taskId);
        if (task is null)
        {
            throw new NotSupportedException($"User with {userEmail} doesn't have task with {taskId} id.");
        }
        return task;
    }
}