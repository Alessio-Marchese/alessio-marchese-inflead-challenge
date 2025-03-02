using Backend.DTO.EXAPI;
using Backend.Entities;

namespace Backend.Mappers.Interfaces
{
    public interface IAddressMapper
    {
        public Address ExapiToDb(ExapiAddressDTO address);
    }
}
