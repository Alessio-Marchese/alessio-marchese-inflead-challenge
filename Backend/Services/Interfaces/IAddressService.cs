using Backend.DTO.EXAPI;
using Backend.Entities;

namespace Backend.Services.Interfaces;

public interface IAddressService
{
    public Task<int> CreateAddressAsync(ExapiAddressDTO address);

    public Address ExapiToDbAddressDTO(ExapiAddressDTO address);
}
