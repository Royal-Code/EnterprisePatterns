using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// Repository for EntityFramework.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TDbContext, TEntity> : IRepository<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{ }
