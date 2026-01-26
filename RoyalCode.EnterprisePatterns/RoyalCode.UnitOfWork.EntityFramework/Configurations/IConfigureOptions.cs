using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// Defines a method to configure options for a <see cref="DbContext"/> using a <see cref="DbContextOptionsBuilder"/>.
/// </summary>
/// <typeparam name="TDbContext">Tipo do DbContext.</typeparam>
public interface IConfigureOptions<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Configures the options for the <see cref="DbContext"/> using the provided <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/> to configure.</param>
    void Configure(DbContextOptionsBuilder optionsBuilder);
}
