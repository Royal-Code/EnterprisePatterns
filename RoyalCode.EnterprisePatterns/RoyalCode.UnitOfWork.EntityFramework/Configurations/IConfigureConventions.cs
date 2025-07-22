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
    /// Configures conventions for the specified <see cref="ModelConfigurationBuilder"/> 
    /// using the provided <typeparamref name="TDbContext"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure conventions for the <typeparamref name="TDbContext"/>.</param>
    void Configure(ModelConfigurationBuilder builder);
}