using Microsoft.EntityFrameworkCore;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories;

/// <summary>
/// Repository for EntityFramework.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TDbContext, TEntity> : IRepository<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{ }
