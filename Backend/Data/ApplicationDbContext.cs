using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
}
