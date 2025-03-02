using Backend.Data;
using Backend.Entities;
using Backend.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Implements;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    //CREATE
    public void CreateUser(User user)
    {
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
    //READ
    public List<User> GetAllUsers()
    {
        return dbContext.Users.ToList();
    }

    public List<User> GetAllUsersWithAddresses()
    {
        return dbContext.Users.Include(u => u.Address).ToList();
    }
}
