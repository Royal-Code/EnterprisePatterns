using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.SmartValidations.Entities;
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
        this.hintPerformer = hintPerformer;
    }

    /// <inheritdoc/>
    public TEntity? Find(object id)
    {
        var entity = db.Set<TEntity>().Find(id);

        if (hintPerformer is not null && entity is not null)
            hintPerformer.Perform<TEntity, DbContext>(entity, db);

        return entity;
    }

    /// <inheritdoc/>
    public async ValueTask<TEntity?> FindAsync(object id, CancellationToken token = default)
    {
        if (id is null)
            return null;

        var entity = await db.Set<TEntity>().FindAsync([ id ], token);

        if (hintPerformer is not null && entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(entity, db);

        return entity;
    }

    /// <inheritdoc/>
    public async ValueTask<Entry<TEntity, TId>> FindAsync<TId>(Id<TEntity, TId> id, CancellationToken token = default)
    {
        var entity = await db.Set<TEntity>().FindAsync([id.Value], token);

        if (hintPerformer is not null && entity is not null)
            await hintPerformer.PerformAsync<TEntity, DbContext>(entity, db);

        return new Entry<TEntity, TId>(entity, id.Value);
    }

    /// <inheritdoc/>
    public void Add(TEntity entity) 
        => db.Set<TEntity>().Add(entity ?? throw new ArgumentNullException(nameof(entity)));

    /// <inheritdoc/>
    public void AddRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        db.Set<TEntity>().AddRange(entities);
    }

    /// <inheritdoc/>
    public async ValueTask AddAsync(TEntity entity, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await db.Set<TEntity>().AddAsync(entity, token);
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
    public async Task<bool> MergeAsync<TId>(IHasId<TId> model, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await FindAsync(model.Id!, token);
        if (entity is null)
            return false;

        var entry = db.Entry(entity);
        entry.CurrentValues.SetValues(model);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MergeAsync<TId, TModel>(Id<TEntity, TId> id, TModel model, CancellationToken token = default)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(model);

        var vEntry = await FindAsync(id, token);
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
    public async Task<TEntity?> DeleteAsync(object id, CancellationToken token = default)
    {
        var entity = await FindAsync(id, token);

        if (entity != null)
            db.Entry(entity).State = EntityState.Deleted;

        return entity;
    }

    /// <summary>
    /// Get queryable for entity, with hints performed if exists.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IQueryable<TEntity> GetQueryable()
    {
        IQueryable<TEntity> query = db.Set<TEntity>();

        if (hintPerformer is not null)
            query = hintPerformer.Perform(query);

        return query;
    }
}