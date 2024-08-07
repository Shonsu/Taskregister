using Taskregister.Server.Exceptions;
using Taskregister.Server.Shared;
using Taskregister.Server.Task.Controller.Dto;
using Taskregister.Server.Task.Contstants;
using Taskregister.Server.Task.Errors;
using Taskregister.Server.Task.Repository;
using Taskregister.Server.Task.Services.Dto;
using Taskregister.Server.User.Errors;
using Taskregister.Server.User.Repository;

namespace Taskregister.Server.Task.Services;

public interface ITaskService
{
    Task<Result<Entities.Task>> GetTaskForUser(string userEmail, int taskId);
    Task<Result<IReadOnlyList<Task.Entities.Task>>> GetTasksForUser(string userEmail, QueryParameters parameters);
    Task<Result<int>> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail);
    System.Threading.Tasks.Task<Result<object>> UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId);
    Task<Result<int>> ChangeTaskState(string userEmail, int taskId, State state);
    Task<Result<int>> ExtendEndDate(string userEmail, int taskId, ExtendBy days);
    System.Threading.Tasks.Task<Result<object>> DeleteTaskAsync(string userEmail, int taskId);
}

public class TaskService(IUserRepository userRepository, ITaskRepository taskRepository) : ITaskService
{
    public async Task<Result<Entities.Task>> GetTaskForUser(string userEmail, int taskId)
    {
        var taskQueryResult = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        //taskQueryResult.Match(onSuccess:t=> Res)
        return taskQueryResult;
    }

    public async Task<Result<IReadOnlyList<Task.Entities.Task>>> GetTasksForUser(string userEmail, QueryParameters parameters)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            //throw new NotFoundException(nameof(User.Entities.User), userEmail);
            return Result<IReadOnlyList<Task.Entities.Task>>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }
        IReadOnlyList<Entities.Task> tasks = await taskRepository.GetAllMatchingTaskForUser(user, parameters);

        return Result<IReadOnlyList<Task.Entities.Task>>.Success(tasks);
    }

    public async Task<Result<int>> CreateTaskAsync(CreateTaskDto createTaskDto, string userEmail)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        var createAt = DateTime.Now;

        if (user is null)
        {
            return Result<int>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }

        DateTime endDate = CalculateEndDate(createTaskDto.Type, createAt);

        var task = new Entities.Task()
        {
            Type = createTaskDto.Type,
            Priority = createTaskDto.Priority,
            CreateAt = createAt,
            EndDate = endDate,
            State = State.New,
            DateState = DateTime.Now,
            Description = createTaskDto.Description
        };

        user.Tasks.Add(task);
        await userRepository.SaveChangesAsync();
        return Result<int>.Success(task.Id);

    }

    public async System.Threading.Tasks.Task<Result<object>> UpdateTaskAsync(UpdateTaskDto updateTaskDto, string userEmail, int taskId)
    {
        
        var taskQueryResult = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        if (!taskQueryResult.IsSuccess)
        {
            return Result<object>.Failure(taskQueryResult.Error);
        }
        var task = taskQueryResult.Value;

        if (task!.State == State.Completed)
        {
            return Result<object>.Failure(TaskErrors.CantModifyCompleted(task.Id));

            //throw new NotSupportedException($"Can't update completed task with {taskId} id.");
        }

        if (task.Type != updateTaskDto.Type)
        {
            task.Type = updateTaskDto.Type;
            task.EndDate = CalculateEndDate(task.Type, task.CreateAt);
        }
        task.Priority = updateTaskDto.Priority;
        task.Description = updateTaskDto.Description;
        await taskRepository.SaveChangesAsync();
        return null;
    }

    public async Task<Result<int>> ChangeTaskState(string userEmail, int taskId, State state)
    {
        // Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        var taskQueryResult = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        if (!taskQueryResult.IsSuccess)
        {
            return Result<int>.Failure(taskQueryResult.Error);
        }
        var task = taskQueryResult.Value;

        State? newState = task.State switch
        {
            State.New or State.Resumed => state == State.Completed ? State.Completed : null,
            State.Completed => state == State.Resumed ? State.Resumed : null,
            _ => null
        };

        if (newState is not null)
        {
            task.State = (State)newState;
            task.DateState = DateTime.Now;
            if (newState == State.Resumed)
            {
                task.EndDate = CalculateEndDate(task.Type, DateTime.Now);
            }
        }
        else
        {
            return Result<int>.Failure(TaskErrors.CantChangeState(taskId, task.State, (State)state));
            throw new NotSupportedException($"Unable to change state of task with {taskId} id with status: {task.State} to {state}.");
        }
        await taskRepository.SaveChangesAsync();
        return Result<int>.Success(task.Id);
    }
    public async Task<Result<int>> ExtendEndDate(string userEmail, int taskId, ExtendBy days)
    {
        // Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        var taskQueryResult = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        if (!taskQueryResult.IsSuccess)
        {
            return Result<int>.Failure(taskQueryResult.Error);
        }
        var task = taskQueryResult.Value;

        if (task!.State == State.Completed)
        {
            return Result<int>.Failure(TaskErrors.CantModifyCompleted(task.Id));
            throw new NotSupportedException($"Can't modify completed task with {taskId} id.");
        }
        task!.EndDate = task!.EndDate.AddDays(days.days);
        task.ChangeEndDateRationale = days.extendByDayRationale;
        task.History.Add($"Extend by {days.days} until {task!.EndDate} - {task.ChangeEndDateRationale}");
        await taskRepository.SaveChangesAsync();
        return Result<int>.Success(task.Id);
    }

    public async System.Threading.Tasks.Task<Result<object>> DeleteTaskAsync(string userEmail, int taskId)
    {
        // Entities.Task task = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        var taskQueryResult = await GetTaskByUserEmailAndTaskId(userEmail, taskId);
        if (!taskQueryResult.IsSuccess)
        {
            return Result<object>.Failure(taskQueryResult.Error);
        }
        var task = taskQueryResult.Value;

        if (task!.State is not State.Completed)
        {
            await taskRepository.Delete(task);
        }
        else
        {
            return Result<object>.Failure(TaskErrors.CantDeleteCompleted(task.Id));
            // throw new NotSupportedException($"Unable to delete task {taskId} id with status: {task.State}.");
        }

        return null;
    }
    
    private static DateTime CalculateEndDate(TaskType type, DateTime createAt)
    {
        return type switch
        {
            TaskType.Type1 => createAt.AddDays((double)TaskType.Type1),
            TaskType.Type2 => createAt.AddDays((double)TaskType.Type2),
            TaskType.Type3 => createAt.AddDays((double)TaskType.Type3),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected task type value: {type}"),
        };
    }

    private async Task<Result<Entities.Task>> GetTaskByUserEmailAndTaskId(string userEmail, int taskId)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            return Result<Entities.Task>.Failure(UserErrors.NotFoundByEmail(userEmail));
            throw new NotFoundException(nameof(User.Entities.User), userEmail);
        }
        var task = await taskRepository.GetTaskByUserIdAndTaskIdAsync(user.Id, taskId);
        if (task is null)
        {
            //throw new NotSupportedException($"User with {userEmail} doesn't have task with {taskId} id.");
            return Result<Entities.Task>.Failure(TaskErrors.NotFoundTaskIdForUserId(user.Id, taskId));
        }
        return Result<Entities.Task>.Success(task);
    }
}