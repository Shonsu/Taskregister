using Taskregister.Server.Exceptions;
using Taskregister.Server.Tags.Controller.Dtos;
using Taskregister.Server.Tags.Entities;
using Taskregister.Server.Tags.Repository;

namespace Taskregister.Server.Tags.Services;

public interface ITagsService
{
    Task<int> CreateTag(Tag tag);
    Task<int> DeleteTag(int tagId);
    Task<int> UpdateTag(int tagId, Tag tag);
    Task<IReadOnlyList<TagDto>> ListTags();

}

public class TagsService(ITagsRepository tagsRepository) : ITagsService
{
    public async Task<IReadOnlyList<TagDto>> ListTags()
    {
        var readOnlyList = await tagsRepository.GetAllAsync();
        return readOnlyList.Select(t => new TagDto(t.Id, t.Name)).ToList();
    }
    
    public async Task<int> CreateTag(Tag tag)
    {
        var tagExist = await tagsRepository.GetByNameAsync(tag.Name);
        if (tagExist is not null)
        {
            throw new ArgumentException($"Tag with name {tag.Name} already exists.");
        }

        await tagsRepository.CreateAsync(tag);
        return tag.Id;
    }
    
    public async Task<int> DeleteTag(int tagId)
    {
        var tagExist = await tagsRepository.GetByIdAsync(tagId);
        if (tagExist is null)
        {
            throw new  NotFoundException(nameof(Tag), tagId.ToString());
        }

        await tagsRepository.DeleteAsync(tagExist);
        return tagExist.Id;
    }
    public async Task<int> UpdateTag(int tagId, Tag tag)
    {
        var tagExist = await tagsRepository.GetByIdAsync(tagId);
        if (tagExist is null)
        {
            throw new  NotFoundException(nameof(Tag), tagId.ToString());
        }
        await tagsRepository.UpdateAsync(tagExist);
        return tagExist.Id;
    }
}