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
    /// Configure as opções do <see cref="DbContext"/> através do <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    /// <param name="optionsBuilder">O <see cref="DbContextOptionsBuilder"/> a ser configurado.</param>
    void Configure(DbContextOptionsBuilder optionsBuilder);
}
