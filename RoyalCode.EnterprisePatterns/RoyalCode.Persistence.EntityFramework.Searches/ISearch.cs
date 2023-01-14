using Microsoft.EntityFrameworkCore;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Searches;

/// <summary>
/// <para>
///     Represents a search for a specific entity type using the Entity Framework Core.
/// </para>    
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ISearch<TDbContext, TEntity> : ISearch<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{ }
