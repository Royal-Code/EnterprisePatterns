using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// Defines a method to configure conventions for a <see cref="DbContext"/> using a <see cref="ModelConfigurationBuilder"/>.
/// </summary>
/// <typeparam name="TDbContext">Tipo do DbContext.</typeparam>
public interface IConfigureConventions<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Configure as convensões do <see cref="DbContext"/> através do <see cref="ModelConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">O <see cref="ModelConfigurationBuilder"/> a ser configurado.</param>
    void Configure(ModelConfigurationBuilder builder);
}