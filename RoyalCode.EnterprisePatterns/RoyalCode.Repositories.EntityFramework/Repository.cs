using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.SmartProblems.Entities;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// <para>
///     Repository pattern implementation using EF.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The EF DbContext type related to the entity.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class Repository<TDbContext, TEntity> : IRepository<TDbContext, TEntity>
    where TEntity: class
    where TDbContext: DbContext
{
    private readonly TDbContext db;
    private readonly DbSet<TEntity> set;
    private readonly IHintPerformer? hintPerformer;

    /// <summary>
    /// <para>
    ///     Creates a new instance of the repository.
    /// </para>
    /// </summary>
    /// <param name="dbContext">The DbContext for work.</param>
    /// <param name="hintPerformer">Optional, hint performer for initialize query with hints.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>
    ///     If the context is null.
    /// </para>
    /// </exception>
    public Repository(TDbContext dbContext, IHintPerformer? hintPerformer = null)
    {
        db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        set = db.Set<TEntity>();
        this.hintPerformer = hintPerformer;
    }

    /// <inheritdoc/>
    public TEntity? Find(object id)
    {
        var entity = set.Find(id);

        if (hintPerformer is not null && entity is not null)
            hintPerformer.Perform<TEntity, DbContext>(entity, db);

        return entity;
    }

    /// <inheritdoc/>
    public async ValueTask<TEntity?> FindAsync(object id, CancellationToken ct = default)
    {
        if (id is null)
            return null;

        var entity = await set.FindAsync([ id ], ct);

        if (hintPerformer is not null && entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(entity, db);

        return entity;
    }

    /// <inheritdoc/>
    public async Task<FindResult<TEntity, TId>> FindAsync<TId>(Id<TEntity, TId> id, CancellationToken ct = default)
    {
        var result = await set.TryFindAsync(id, ct);

        if (hintPerformer is not null && result.Entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(result.Entity, db);

        return result;
    }

    /// <inheritdoc/>
    public Task<FindResult<TDto, TId>> FindAsync<TDto, TId>(Id<TEntity, TId> id, CancellationToken ct = default)
        where TDto : class
    {
        return db.TryFindAsync<TEntity, TDto, TId>(id, ct);
    }

    /// <inheritdoc/>
    public async Task<FindResult<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        var result = await set.TryFindByAsync(filter, ct);

        if (hintPerformer is not null && result.Entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(result.Entity, db);

        return result;
    }

    /// <inheritdoc/>
    public async Task<FindResult<TEntity>> FindAsync<TValue>(
        Expression<Func<TEntity, TValue>> propertySelector,
        TValue filterValue,
        CancellationToken ct = default)
    {
        var result = await set.TryFindByAsync(propertySelector, filterValue, ct);

        if (hintPerformer is not null && result.Entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(result.Entity, db);

        return result;
    }

    /// <inheritdoc/>
    public void Add(TEntity entity) 
        => set.Add(entity ?? throw new ArgumentNullException(nameof(entity)));

    /// <inheritdoc/>
    public void AddRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        set.AddRange(entities);
    }

    /// <inheritdoc/>
    public async ValueTask AddAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await set.AddAsync(entity, ct);
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (var entity in entities)
        {
            await set.AddAsync(entity, ct);
        }
    }

    /// <inheritdoc/>
    public bool Merge<TId>(IHasId<TId> model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = Find(model.Id!);
        if (entity is null)
            return false;

        var entry = db.Entry(entity);
        entry.CurrentValues.SetValues(model);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MergeAsync<TId>(IHasId<TId> model, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await FindAsync(model.Id!, ct);
        if (entity is null)
            return false;

        var entry = db.Entry(entity);
        entry.CurrentValues.SetValues(model);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MergeAsync<TId, TModel>(Id<TEntity, TId> id, TModel model, CancellationToken ct = default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(model);

        var vEntry = await FindAsync(id, ct);
        if (vEntry.NotFound(out _))
            return false;

        var dbEntry = db.Entry(vEntry.Entity);
        dbEntry.CurrentValues.SetValues(model);

        return true;
    }

    /// <inheritdoc/>
    public void Remove(TEntity entity)
    {
        var entry = db.Entry(entity);
        entry.State = EntityState.Deleted;
    }

    /// <inheritdoc/>
    public TEntity? Delete(object id)
    {
        var entity = Find(id);

        if (entity is not null)
            db.Entry(entity).State = EntityState.Deleted;

        return entity;
    }

    /// <inheritdoc/>
    public async Task<TEntity?> DeleteAsync(object id, CancellationToken ct = default)
    {
        var entity = await FindAsync(id, ct);

        if (entity != null)
            db.Entry(entity).State = EntityState.Deleted;

        return entity;
    }

    /// <inheritdoc/>
    public async Task<FindResult<TEntity, TId>> DeleteAsync<TId>(Id<TEntity, TId> id, CancellationToken ct = default)
    {
        var result = await set.TryFindAsync(id, ct);

        if (result.Entity is not null)
            db.Entry(result.Entity).State = EntityState.Deleted;

        return result;
    }

    /// <summary>
    /// Get queryable for entity, with hints performed if exists.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IQueryable<TEntity> GetQueryable()
    {
        IQueryable<TEntity> query = set;

        if (hintPerformer is not null)
            query = hintPerformer.Perform(query);

        return query;
    }
}