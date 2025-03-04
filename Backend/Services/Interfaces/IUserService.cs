using Backend.DTO.MYAPI;
using Backend.Entities;
using Backend.Utility;

namespace Backend.Services.Interfaces;

public interface IUserService
{
    public Task<List<User>> GetAllUsersWithAddressesAsync();
    public bool CheckIfIsSingleResult(string? email, string? username);
    public Task<Result<MyApiUserDTO>> FindSingleUser(string? gender, string? email, string? username, List<User> dbUsers);
    public Task<Result<List<MyApiUserDTO>>> FindManyUsers(string? gender, string? email, string? username, List<User> dbUsers);
}
