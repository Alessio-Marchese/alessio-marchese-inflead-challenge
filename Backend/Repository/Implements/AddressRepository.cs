using Backend.Data;
using Backend.Entities;
using Backend.Repository.Interfaces;

namespace Backend.Repository.Implements;

public class AddressRepository(ApplicationDbContext dbContext) : IAddressRepository
{
    //CREATE
    public void CreateAddress(Address address)
    {
        dbContext.Addresses.Add(address);
        dbContext.SaveChanges();
    }
}
