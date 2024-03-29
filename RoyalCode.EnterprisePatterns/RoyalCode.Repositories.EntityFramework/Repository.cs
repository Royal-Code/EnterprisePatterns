using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;
using RoyalCode.OperationHint.Abstractions;
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
            hintPerformer.Perform<TEntity, DbContext>(entity, db);

        return entity;
    }
    
    /// <inheritdoc/>
    public void Add(TEntity entity) 
        => db.Set<TEntity>()
            .Add(entity ?? throw new ArgumentNullException(nameof(entity)));

    /// <inheritdoc/>
    public void AddRange(IEnumerable<TEntity> entities)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities));

        db.Set<TEntity>().AddRange(entities);
    }

    /// <inheritdoc/>
    public bool Merge<TId>(IHasId<TId> model)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

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
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        var entity = await FindAsync(model.Id!, token);
        if (entity is null)
            return false;

        var entry = db.Entry(entity);
        entry.CurrentValues.SetValues(model);

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