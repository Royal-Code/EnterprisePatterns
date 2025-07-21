using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// Defines a contract for configuring the <see cref="ModelBuilder"/> of a <see cref="DbContext"/>.
/// </summary>
public interface IConfigureModel<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Configura o <see cref="ModelBuilder"/> de um <see cref="DbContext"/>.
    /// </summary>
    /// <param name="modelBuilder"></param>
    public void Configure(ModelBuilder modelBuilder);
}
