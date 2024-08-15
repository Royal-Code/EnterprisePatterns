using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// <para>
///     Implementation of <see cref="IFinderByGuid{TEntity}"/> using EF.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The EF DbContext type related to the entity.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class FinderByGuid<TDbContext, TEntity> : IFinderByGuid<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IHasGuid
{
    private readonly TDbContext db;
    
    /// <summary>
    /// Creates a new instance of the finder.
    /// </summary>
    /// <param name="dbContext">The DbContext for work.</param>
    public FinderByGuid(TDbContext dbContext)
    {
        db = dbContext;
    }
        
    /// <inheritdoc/>
    public TEntity? FindByGuid(Guid guid)
    {
        return db.Set<TEntity>().FirstOrDefault(e => e.Guid == guid);
    }

    /// <inheritdoc/>
    public Task<TEntity?> FindByGuidAsync(Guid guid, CancellationToken token = default)
    {
        return db.Set<TEntity>().FirstOrDefaultAsync(e => e.Guid == guid, token);
    }
}