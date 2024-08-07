using Microsoft.EntityFrameworkCore;
using Taskregister.Server.Persistence;

namespace Taskregister.Server.User.Repository;

public interface IUserRepository
{
    Task<int> CreateUserAsync(Entities.User user);
    Task<IEnumerable<Entities.User>> GetAllUsersAsync();
    Task<Entities.User?> GetUserAsync(string email);
    Task<int> GetUserIdByEmailAsync(string userEmail);
    Task SaveChangesAsync();
}

public class UserRepository(TodosRegisterDbContext dbContext) : IUserRepository
{
    public async Task<int> CreateUserAsync(Entities.User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user.Id;
    }
    public async Task<IEnumerable<Entities.User>> GetAllUsersAsync()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task<Entities.User?> GetUserAsync(string email)
    {
        return await dbContext.Users.Where(u=>u.Email.Equals(email)).SingleOrDefaultAsync(); // Include(u=>u.Tasks).
    }

    public async Task<int> GetUserIdByEmailAsync(string userEmail)
    {
        return await dbContext.Users.Where(u=>u.Email.Equals(userEmail)).Select(u=>u.Id).SingleOrDefaultAsync();
    }

    public async Task SaveChangesAsync()=>await dbContext.SaveChangesAsync();

}
