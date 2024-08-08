using Microsoft.EntityFrameworkCore;
using Taskregister.Server.Exceptions;
using Taskregister.Server.Persistence;
using Taskregister.Server.Tags.Entities;

namespace Taskregister.Server.Tags.Repository;

public interface ITagsRepository
{
    Task<int> CreateAsync(Tag tag);
    Task<int> DeleteAsync(Tag tag);
    Task<int> UpdateAsync(Tag tag);
    Task<IReadOnlyList<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag?> GetByNameAsync(string value);
}

public class TagsRepository(TodosRegisterDbContext dbContext) : ITagsRepository
{
    public async Task<int> CreateAsync(Tag tag)
    {
        await dbContext.Tags.AddAsync(tag);
        await dbContext.SaveChangesAsync();
        return tag.Id;
    }

    public async Task<int> DeleteAsync(Tag tag)
    {
        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync();
        return tag.Id;
    }

    public async Task<int> UpdateAsync(Tag tag)
    {
        // var tagToUpdate = await dbContext.Tags.FindAsync(tag.Id);
        // if (tagToUpdate == null)
        // {
        //     throw new  NotFoundException(nameof(Tag), tag.Id.ToString());
        // }
        //
        // dbContext.Entry(tagToUpdate).CurrentValues.SetValues(tag);
        dbContext.Tags.Update(tag);
        await dbContext.SaveChangesAsync();
        return tag.Id;
    }

    public async Task<IReadOnlyList<Tag>> GetAllAsync()
    {
        return await dbContext.Tags.ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await dbContext.Tags.FindAsync(id);
    }
    public async Task<Tag?> GetByNameAsync(string value)
    {
        return await dbContext.Tags.SingleOrDefaultAsync(t=>t.Name.Equals(value));
    }
}