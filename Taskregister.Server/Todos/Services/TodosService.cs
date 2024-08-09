using Taskregister.Server.Shared;
using Taskregister.Server.Tags.Repository;
using Taskregister.Server.Todos.Constants;
using Taskregister.Server.Todos.Controller.Dto;
using Taskregister.Server.Todos.Contstants;
using Taskregister.Server.Todos.Entities;
using Taskregister.Server.Todos.Errors;
using Taskregister.Server.Todos.Repository;
using Taskregister.Server.User.Errors;
using Taskregister.Server.User.Repository;

namespace Taskregister.Server.Todos.Services;

public interface ITodosService
{
    Task<Result<Todo>> GetTodoForUser(string userEmail, int todoId);
    Task<Result<IReadOnlyList<Todo>>> GetTodosForUser(string userEmail, QueryParameters parameters);
    Task<Result<int>> CreateTodoAsync(CreateTodoDto createTodoDto, string userEmail);
    Task<Result> UpdateTodoAsync(UpdateTodoDto updateTodoDto, string userEmail, int todoId);
    Task<Result<int>> ChangeTodoState(string userEmail, int todoId, State state);
    Task<Result<int>> ExtendTodoEndDate(string userEmail, int todoId, ExtendBy days);
    Task<Result> DeleteTodoAsync(string userEmail, int todoId);
}

public class TodosService(
    IUserRepository userRepository,
    ITodosRepository todosRepository,
    ITagsRepository tagsRepository) : ITodosService
{
    public async Task<Result<Todo>> GetTodoForUser(string userEmail, int todoId)
    {
        var todoQueryResult = await GetTodoByUserEmailAndTodoId(userEmail, todoId);
        return todoQueryResult;
    }

    public async Task<Result<IReadOnlyList<Todo>>> GetTodosForUser(string userEmail, QueryParameters parameters)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            return Result<IReadOnlyList<Todo>>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }

        IReadOnlyList<Todo> todos = await todosRepository.GetAllMatchingTodoForUser(user, parameters);

        return Result<IReadOnlyList<Todo>>.Success(todos);
    }

    public async Task<Result<int>> CreateTodoAsync(CreateTodoDto createTodoDto, string userEmail)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        var createAt = DateTime.Now;

        if (user is null)
        {
            return Result<int>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }

        // TODO check if tagIds exist in DB
        var tags = await tagsRepository.GetByIdListAsync(createTodoDto.TagIds);

        DateTime endDate = CalculateEndDate(createTodoDto.Type, createAt);

        var todo = new Todo
        {
            Type = createTodoDto.Type,
            Priority = createTodoDto.Priority,
            CreateAt = createAt,
            EndDate = endDate,
            State = State.New,
            DateState = DateTime.Now,
            Description = createTodoDto.Description,
            Tags = tags.ToList()
        };

        user.Todos.Add(todo);
        await userRepository.SaveChangesAsync();
        return Result<int>.Success(todo.Id);
    }

    public async Task<Result> UpdateTodoAsync(UpdateTodoDto updateTodoDto, string userEmail, int todoId)
    {
        var todoQueryResult = await GetTodoByUserEmailAndTodoId(userEmail, todoId);
        if (!todoQueryResult.IsSuccess)
        {
            return Result.Failure(todoQueryResult.Error);
        }

        var todo = todoQueryResult.Value;

        if (todo!.State == State.Completed)
        {
            return Result.Failure(TodoErrors.CantModifyCompleted(todo.Id));
        }

        if (todo.Type != updateTodoDto.Type)
        {
            todo.Type = updateTodoDto.Type;
            todo.EndDate = CalculateEndDate(todo.Type, todo.CreateAt);
        }

        todo.Priority = updateTodoDto.Priority;
        todo.Description = updateTodoDto.Description;
        var tags = await tagsRepository.GetByIdListAsync(updateTodoDto.TagIds);
        if(tags is not null)
        {
            //var list = todo.Tags.Where(t => !tags.Contains(t)).ToList();
            todo.Tags.RemoveAll(t => !tags.Contains(t));
            todo.Tags.AddRange(tags);
        }
        await todosRepository.SaveChangesAsync();
        return Result.Success();;
    }

    public async Task<Result<int>> ChangeTodoState(string userEmail, int todoId, State state)
    {
        var todoQueryResult = await GetTodoByUserEmailAndTodoId(userEmail, todoId);
        if (!todoQueryResult.IsSuccess)
        {
            return Result<int>.Failure(todoQueryResult.Error);
        }

        var todo = todoQueryResult.Value;

        State? newState = todo.State switch
        {
            State.New or State.Resumed => state == State.Completed ? State.Completed : null,
            State.Completed => state == State.Resumed ? State.Resumed : null,
            _ => null
        };

        if (newState is not null)
        {
            todo.State = (State)newState;
            todo.DateState = DateTime.Now;
            if (newState == State.Resumed)
            {
                todo.EndDate = CalculateEndDate(todo.Type, DateTime.Now);
            }
        }
        else
        {
            return Result<int>.Failure(TodoErrors.CantChangeState(todoId, todo.State, state));
        }

        await todosRepository.SaveChangesAsync();
        return Result<int>.Success(todo.Id);
    }

    public async Task<Result<int>> ExtendTodoEndDate(string userEmail, int todoId, ExtendBy days)
    {
        var todoQueryResult = await GetTodoByUserEmailAndTodoId(userEmail, todoId);
        if (!todoQueryResult.IsSuccess)
        {
            return Result<int>.Failure(todoQueryResult.Error);
        }

        var todo = todoQueryResult.Value;

        if (todo!.State == State.Completed)
        {
            return Result<int>.Failure(TodoErrors.CantModifyCompleted(todo.Id));
        }

        todo!.EndDate = todo!.EndDate.AddDays(days.days);
        todo.ChangeEndDateRationale = days.extendByDayRationale;
        todo.History.Add($"Extend by {days.days} until {todo!.EndDate} - {todo.ChangeEndDateRationale}");
        await todosRepository.SaveChangesAsync();
        return Result<int>.Success(todo.Id);
    }

    public async Task<Result> DeleteTodoAsync(string userEmail, int todoId)
    {
        var todoQueryResult = await GetTodoByUserEmailAndTodoId(userEmail, todoId);
        if (!todoQueryResult.IsSuccess)
        {
            return Result.Failure(todoQueryResult.Error);
        }

        var todo = todoQueryResult.Value;

        if (todo!.State is not State.Completed)
        {
            await todosRepository.Delete(todo);
        }
        else
        {
            return Result.Failure(TodoErrors.CantDeleteCompleted(todo.Id));
        }

        return Result.Success();;
    }

    private static DateTime CalculateEndDate(TodoType type, DateTime createAt)
    {
        return type switch
        {
            TodoType.Type1 => createAt.AddDays((double)TodoType.Type1),
            TodoType.Type2 => createAt.AddDays((double)TodoType.Type2),
            TodoType.Type3 => createAt.AddDays((double)TodoType.Type3),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected task type value: {type}"),
        };
    }

    private async Task<Result<Todo>> GetTodoByUserEmailAndTodoId(string userEmail, int todoId)
    {
        var user = await userRepository.GetUserAsync(userEmail);
        if (user is null)
        {
            return Result<Todo>.Failure(UserErrors.NotFoundByEmail(userEmail));
        }

        var todo = await todosRepository.GetTodoByUserIdAndTodoIdAsync(user.Id, todoId);
        if (todo is null)
        {
            return Result<Todo>.Failure(TodoErrors.NotFoundTodoIdForUserId(user.Id, todoId));
        }
        var tags = todo.Tags;

        return Result<Todo>.Success(todo);
    }
}