using Backend.DTO.EXAPI;
using Backend.DTO.MYAPI;
using Backend.Entities;

namespace Backend.Mappers.Interfaces
{
    public interface IUserMapper
    {
        public MyApiUserDTO DbToMyApiDTO(User user);

        public MyApiUserDTO ExapiToMyApiDTO(ExapiUserDTO user);

        public User ExapiToDb(ExapiUserDTO user);
    }
}
