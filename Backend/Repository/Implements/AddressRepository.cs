using Backend.Data;
using Backend.Entities;
using Backend.Repository.Interfaces;

namespace Backend.Repository.Implements;

public class AddressRepository(ApplicationDbContext dbContext) : IAddressRepository
{
    //CREATE
    public async Task<int> CreateAddressAsync(Address address)
    {
        await dbContext.Addresses.AddAsync(address);
        await dbContext.SaveChangesAsync();
        return address.Id;
    }
}
