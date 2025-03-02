using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;

namespace Backend.Services.Interfaces;

public interface IUserService
{
    public Task CreateUserAsync(ExapiUserDTO user, int addressId);
    public Task<List<User>> GetAllUsersAsync();
    public Task<List<User>> GetAllUsersWithAddressesAsync();
    public MyApiUserDTO ExapiToMyApiDTO(ExapiUserDTO user);
    public MyApiUserDTO DbToMyApiDTO(User user);
    public User ExapiToDbDTO(ExapiUserDTO user);
}
