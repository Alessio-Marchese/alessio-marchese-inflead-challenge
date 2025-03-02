using Backend.Data;
using Backend.Entities;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implements;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    //CREATE
    public async Task CreateUserAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
    //READ
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task<List<User>> GetAllUsersWithAddressesAsync()
    {
        return await dbContext.Users.Include(u => u.Address).ToListAsync();
    }
}
