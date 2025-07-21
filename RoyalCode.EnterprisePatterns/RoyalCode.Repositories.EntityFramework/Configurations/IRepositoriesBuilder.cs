using Microsoft.EntityFrameworkCore;
using RoyalCode.Repositories.Configurations;

namespace RoyalCode.Repositories.EntityFramework.Configurations;

/// <summary>
///    Configure repositories for the unit of work.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public interface IRepositoriesBuilder<out TDbContext> : IRepositoriesBuilder
    where TDbContext : DbContext
{ }
