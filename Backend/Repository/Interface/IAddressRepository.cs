using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Interface;

public interface IAddressRepository
{
    //CREATE
    public void CreateAddress(Address address);
}
