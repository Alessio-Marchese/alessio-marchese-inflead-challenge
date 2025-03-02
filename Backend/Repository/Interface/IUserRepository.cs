using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Interface;

public interface IUserRepository
{
    //CREATE
    public void CreateUser(User user);
    //READ
    public List<User> GetAllUsers();

    public List<User> GetAllUsersWithAddresses();
}
