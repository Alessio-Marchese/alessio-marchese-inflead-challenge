using Backend.DTO.EXAPI;
using Backend.Entities;
using Backend.Mappers.Interfaces;
using Backend.Repository.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implements;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IAddressMapper _addressMapper;
    public AddressService(IAddressRepository addressRepository, IAddressMapper addressMapper)
    {
        _addressRepository = addressRepository;
        _addressMapper = addressMapper;
    }

    public async Task<int> CreateAddressAsync(ExapiAddressDTO address)
    {
        return await _addressRepository.CreateAddressAsync(_addressMapper.ExapiToDb(address));
    }

    public Address ExapiToDbAddressDTO(ExapiAddressDTO address)
    {
        return _addressMapper.ExapiToDb(address);
    }
}
