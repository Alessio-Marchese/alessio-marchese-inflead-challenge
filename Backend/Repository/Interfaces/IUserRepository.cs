using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Interfaces;

public interface IUserRepository
{
    //CREATE
    public void CreateUser(User user);
    //READ
    public List<User> GetAllUsers();

    public List<User> GetAllUsersWithAddresses();
}
