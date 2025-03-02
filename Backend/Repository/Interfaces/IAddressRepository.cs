using Backend.Entities;

namespace Backend.Repository.Interfaces;

public interface IAddressRepository
{
    //CREATE
    public Task<int> CreateAddressAsync(Address address);
}
