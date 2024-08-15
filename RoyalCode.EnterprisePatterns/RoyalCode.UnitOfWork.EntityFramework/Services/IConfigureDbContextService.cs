using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Services;

/// <summary>
/// A service that configures a <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public interface IConfigureDbContextService<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Applies the configuration to the <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/>.</param>
    public void ConfigureDbContext(DbContextOptionsBuilder builder);
}