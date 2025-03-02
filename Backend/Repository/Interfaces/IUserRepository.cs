using Backend.Entities;

namespace Backend.Repository.Interfaces;

public interface IUserRepository
{
    //CREATE
    public Task CreateUserAsync(User user);
    //READ
    public Task<List<User>> GetAllUsersAsync();

    public Task<List<User>> GetAllUsersWithAddressesAsync();
}
