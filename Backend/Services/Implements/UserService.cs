using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;
using Backend.Mappers.Interfaces;
using Backend.Repository.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _userMapper;
    public UserService(IUserRepository userRepository, IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task CreateUserAsync(ExapiUserDTO user, int addressId)
    {
        user.AddressId = addressId;
        await _userRepository.CreateUserAsync(_userMapper.ExapiToDb(user));
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<List<User>> GetAllUsersWithAddressesAsync()
    {
        return await _userRepository.GetAllUsersWithAddressesAsync();
    }

    public MyApiUserDTO ExapiToMyApiDTO(ExapiUserDTO user)
    {
        return _userMapper.ExapiToMyApiDTO(user);
    }

    public MyApiUserDTO DbToMyApiDTO(User user)
    {
        return _userMapper.DbToMyApiDTO(user);
    }

    public User ExapiToDbDTO(ExapiUserDTO user)
    {
        return _userMapper.ExapiToDb(user);
    }
}
