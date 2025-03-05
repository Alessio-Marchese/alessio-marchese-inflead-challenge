using Backend.DTO.MYAPI;
using Backend.Utility;

namespace Backend.Services.Interfaces;

public interface IUserService
{
    public bool CheckIfIsSingleResult(string? email, string? username);
    public Task<Result<MyApiUserDTO>> FindSingleUser(string? email, string? username);
    public Task<Result<List<MyApiUserDTO>>> FindManyUsers(string? gender);
}
