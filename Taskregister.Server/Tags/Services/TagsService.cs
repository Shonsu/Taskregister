using Taskregister.Server.Exceptions;
using Taskregister.Server.Shared;
using Taskregister.Server.Tags.Controller.Dtos;
using Taskregister.Server.Tags.Entities;
using Taskregister.Server.Tags.Errors;
using Taskregister.Server.Tags.Repository;
using Taskregister.Server.Todos.Repository;

namespace Taskregister.Server.Tags.Services;

public interface ITagsService
{
    Task<Result<int>> CreateTag(Tag tag);
    Task<Result<int>> DeleteTag(int tagId);
    Task<Result<int>> UpdateTag(int tagId, Tag tag);
    Task<IReadOnlyList<TagDto>> ListTags();
    Task<Result<Tag?>> GetTagById(int tagId);

}

public class TagsService(ITagsRepository tagsRepository, ITodosRepository todosRepository) : ITagsService
{
    public async Task<IReadOnlyList<TagDto>> ListTags()
    {
        var readOnlyList = await tagsRepository.GetAllAsync();
        return readOnlyList.Select(t => new TagDto(t.Id, t.Name)).ToList();
    }

    public async Task<Result<Tag?>> GetTagById(int tagId)
    {
        var tag = await tagsRepository.GetByIdAsync(tagId);
        if (tag is null)
        {
            return Result<Tag?>.Failure(TagErrors.NotFoundTagWithId(tagId));
        }
        return Result<Tag?>.Success(tag);
    }

    public async Task<Result<int>> CreateTag(Tag tag)
    {
        var tagExist = await tagsRepository.GetByNameAsync(tag.Name);
        if (tagExist is not null)
        {
            return Result<int>.Failure(TagErrors.TagNameAlreadyExist(tag.Name));
        }

        await tagsRepository.CreateAsync(tag);
        return Result<int>.Success(tag.Id);
    }
    
    public async Task<Result<int>> DeleteTag(int tagId)
    {
        var tagExist = await tagsRepository.GetByIdAsync(tagId);
       
        if (tagExist is null)
        {
            return Result<int>.Failure(TagErrors.NotFoundTagWithId(tagId));
        }

        var todoExistByTag = await todosRepository.TodoExistByTag(tagExist);
        if (todoExistByTag)
        {
            return Result<int>.Failure(TagErrors.AlreadyAssignedToTodo(tagId));
        }
        
        await tagsRepository.DeleteAsync(tagExist);
        return Result<int>.Success(tagExist.Id);
    }
    public async Task<Result<int>> UpdateTag(int tagId, Tag tag)
    {
        var tagExist = await tagsRepository.GetByIdAsync(tagId);
        if (tagExist is null)
        {
            return Result<int>.Failure(TagErrors.NotFoundTagWithId(tagId));
        }
        var tagNameExist = await tagsRepository.GetByNameAsync(tag.Name);
        if (tagNameExist is not null)
        {
            return Result<int>.Failure(TagErrors.TagNameAlreadyExist(tag.Name));
        }
        tagExist.Name = tag.Name;
        await tagsRepository.UpdateAsync(tagExist);
        return Result<int>.Success(tagExist.Id);
    }
}