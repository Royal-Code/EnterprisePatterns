using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Commands.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) 
        : base(options)
    { }
}
