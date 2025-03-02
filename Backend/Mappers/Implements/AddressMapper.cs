using Backend.DTO.EXAPI;
using Backend.Entities;
using Backend.Mappers.Interfaces;

namespace Backend.Mappers.Implements;

public class AddressMapper : IAddressMapper
{
    public Address ExapiToDb(ExapiAddressDTO address)
    {
        return new Address()
        {
            City = address.City,
            Street = $"{address.StreetName} {address.StreetAddress}",
            ZipCode = address.ZipCode,
            State = address.State
        };
    }
}
