using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Examples.Api.EF;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
