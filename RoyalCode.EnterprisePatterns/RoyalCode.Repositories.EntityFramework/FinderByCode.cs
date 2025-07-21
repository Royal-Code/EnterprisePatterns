using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// <para>
///     Default implementation of <see cref="IFinderByCode{TEntity,TCode}"/> using EF.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The EF DbContext type related to the entity.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TCode">The type of the code.</typeparam>
public class FinderByCode<TDbContext, TEntity, TCode> : IFinderByCode<TEntity, TCode>
    where TDbContext : DbContext
    where TEntity : class, IHasCode<TCode>
{
    private readonly TDbContext db;
    
    /// <summary>
    /// Creates a new instance of the <see cref="FinderByCode{TDbContext,TEntity,TCode}"/>
    /// </summary>
    /// <param name="dbContext">The DbContext for work.</param>
    public FinderByCode(TDbContext dbContext)
    {
        db = dbContext;
    }
    
    /// <inheritdoc/>
    public TEntity? FindByCode(TCode code)
    {
        return db.Set<TEntity>().FirstOrDefault(e => e.Code!.Equals(code));
    }

    /// <inheritdoc/>
    public Task<TEntity?> FindByCodeAsync(TCode code, CancellationToken token = default)
    {
        return db.Set<TEntity>().FirstOrDefaultAsync(e => e.Code!.Equals(code), token);
    }
}