using Azure.Identity;
using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;

namespace Backend.Mappers;

public class UserMapper
{
    public static MyApiUserDTO DbToMyApiDTO(User user)
    {
        return new MyApiUserDTO()
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            ProfilePicUrl = user.ProfilePicUrl,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            Employment = user.Employment,
            KeySkill = user.KeySkill,
            AddressId = user.Address.Id.ToString(),
            CreationDate = user.CreationDate
        };
    }

    public static MyApiUserDTO ExapiToMyApiDTO(ExapiUserDTO user)
    {
        return new MyApiUserDTO()
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Username = user.Username,
            FullName = $"{user.FirstName} {user.LastName}",
            ProfilePicUrl = user.ProfilePicUrl,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            Employment = $"{user.Employment.Title} {user.Employment.KeySkill}",
            KeySkill = user.Employment.KeySkill,
            CreationDate = DateTime.Now
        };

    }

    public static User ExapiToDb(ExapiUserDTO user)
    {
        return new User()
        {
            Uid = user.Uid,
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = $"{user.FirstName} {user.LastName}",
            ProfilePicUrl = user.ProfilePicUrl,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            Employment = $"{user.Employment.Title} {user.Employment.KeySkill}",
            KeySkill = user.Employment.KeySkill,
            Address = new Address()
            {
                City = user.Address.City,
                Street = $"{user.Address.StreetName} {user.Address.StreetAddress}",
                ZipCode = user.Address.ZipCode,
                State = user.Address.State
            }
        };

    }
}
