using Taskregister.Server.Exceptions;
using Taskregister.Server.Shared;
using Taskregister.Server.Task.Controller.Dto;
using Taskregister.Server.Task.Contstants;
using Taskregister.Server.Task.Repository;
using Taskregister.Server.Task.Services.Dto;
using Taskregister.Server.User.Errors;
using Taskregister.Server.User.Repository;

namespace Taskregister.Server.Task.Services;

public interface ITaskService
{
    Task<int> ChangeTaskState(string userEmail, int taskId, State state);

    Task<int> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail);

    System.Threading.Tasks.Task DeleteTaskAsync(string userEmail, int taskId);

    Task<int> ExtendEndDate(string userEmail, int taskId, ExtendBy days);

    Task<Entities.Task> GetTaskForUser(string userEmail, int taskId);

    Task<Result<IEnumerable<Task.Entities.Task>>> GetTasksForUser(string userEmail, QueryParameters parameters);

    System.Threading.Tasks.Task UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId);
}

public class TaskService(IUserRepository userRepository, ITaskRepository taskRepository) : ITaskService
{
    public async Task<Entities.Task> GetTaskForUser(string userEmail, int taskId)
    {
        return await GetTaskByUserEmailAndTaskId(userEmail, taskId);
    }

    public async Task<Result<IEnumerable<Task.Entities.Task>>> GetTasksForUser(string userEmail, QueryParameters parameters)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            //throw new NotFoundException(nameof(User.Entities.User), userEmail);
            return Result<IEnumerable<Task.Entities.Task>>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }
        IEnumerable<Entities.Task> tasks = await taskRepository.GetAllMatchingTaskForUser(user, parameters);

        return Result<IEnumerable<Task.Entities.Task>>.Success(tasks);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId)
    {
        Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);

        if (task!.State == State.COMPLETED)
        {
            throw new NotSupportedException($"Can't update completed task with {taskId} id.");
        }

        if (task.Type != updateTaskDto.Type)
        {
            task.Type = updateTaskDto.Type;
            task.EndDate = CalculateEndDate(task.Type, task.CreateAt);
        }
        task.Priority = updateTaskDto.Priority;
        task.Description = updateTaskDto.Description;
        await taskRepository.SaveChangesAsync();
    }

    public async Task<int> ChangeTaskState(string userEmail, int taskId, State state)
    {
        Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);

        State? newState = task.State switch
        {
            State.NEW or State.RESUMED => state == State.COMPLETED ? State.COMPLETED : null,
            State.COMPLETED => state == State.RESUMED ? State.RESUMED : null,
            _ => null
        };
        
        if (newState is not null)
        {
            task.State = (State)newState;
            task.DateState = DateTime.Now;
            if (newState == State.RESUMED)
            {
                task.EndDate = CalculateEndDate(task.Type, DateTime.Now);
            }
        }
        else
        {
            throw new NotSupportedException($"Unable to change state of task with {taskId} id with status: {task.State} to {state}.");
        }
        await taskRepository.SaveChangesAsync();
        return task.Id;
    }

    public async Task<int> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        var createAt = DateTime.Now;

        if (user is null)
        {
            throw new NotFoundException(nameof(User.Entities.User), userEmail);
        }

        DateTime endDate = CalculateEndDate(createTaskDto.Type, createAt);

        var task = new Entities.Task()
        {
            Type = createTaskDto.Type,
            Priority = createTaskDto.Priority,
            CreateAt = createAt,
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
        Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);

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
        Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        if (task!.State == State.COMPLETED)
        {
            throw new NotSupportedException($"Can't modify completed task with {taskId} id.");
        }
        task!.EndDate = task!.EndDate.AddDays(days.days);
        task.ChangeEndDateRationale = days.extendByDayRationale;
        task.History.Add($"Extend by {days.days} until {task!.EndDate} - {task.ChangeEndDateRationale}");
        await taskRepository.SaveChangesAsync();
        return task.Id;
    }
    private static DateTime CalculateEndDate(TaskType type, DateTime createAt)
    {
        return type switch
        {
            TaskType.TYPE_1 => createAt.AddDays((double)TaskType.TYPE_1),
            TaskType.TYPE_2 => createAt.AddDays((double)TaskType.TYPE_2),
            TaskType.TYPE_3 => createAt.AddDays((double)TaskType.TYPE_3),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected task type value: {type}"),
        };
    }

    private async Task<Entities.Task> GetTaskByUserEmailAndTaskId(string userEmail, int taskId)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            throw new NotFoundException(nameof(User.Entities.User), userEmail);
        }
        var task = await taskRepository.GetTaskByUserIdAndTaskIdAsync(user.Id, taskId);
        if (task is null)
        {
            throw new NotSupportedException($"User with {userEmail} doesn't have task with {taskId} id.");
        }
        return task;
    }
}