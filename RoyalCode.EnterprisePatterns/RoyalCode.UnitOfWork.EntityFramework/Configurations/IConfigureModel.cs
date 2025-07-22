using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// Defines a contract for configuring the <see cref="ModelBuilder"/> of a <see cref="DbContext"/>.
/// </summary>
public interface IConfigureModel<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Configures the <see cref="ModelBuilder"/> for the specified <see cref="DbContext"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
    public void Configure(ModelBuilder modelBuilder);
}
