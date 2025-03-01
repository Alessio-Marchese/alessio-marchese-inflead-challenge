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
            AddressId = user.Address.Id
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
            KeySkill = user.Employment.KeySkill
            //Manca l'id dell'indirizzo
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
            KeySkill = user.Employment.KeySkill
        };
    }
}
