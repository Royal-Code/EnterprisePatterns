using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RoyalCode.OperationHint.Abstractions;
using System.Linq.Expressions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Hints;

public abstract class EntityFrameworkHintHandler<TEntity, THint> : IHintQueryHandler<IQueryable<TEntity>, THint>, IHintEntityHandler<TEntity, DbContext, THint>
    where TEntity : class
    where THint : Enum
{
    public IQueryable<TEntity> Handle(IQueryable<TEntity> query, THint hint)
    {
        throw new NotImplementedException();
    }

    public void Handle(TEntity entity, DbContext source, THint hint)
    {
        var entry = source.Entry(entity);
        throw new NotImplementedException();
    }

    public Task HandleAsync(TEntity entity, DbContext source, THint hint)
    {
        throw new NotImplementedException();
    }

    // protected 
}

public abstract class Expressions<TEntity> where TEntity : class
{
    public abstract Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty?>> expression)
        where TProperty: class;

    public abstract Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression)
        where TProperty : class;
}

internal sealed class QueryableIncludes<TEntity> : Expressions<TEntity> where TEntity : class
{
    private IQueryable<TEntity> query;

    public QueryableIncludes(IQueryable<TEntity> query)
    {
        this.query = query;
    }

    public override Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty?>> expression)
        where TProperty : class
    {
        query = query.Include(expression);
        return this;
    }

    public override Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class
    {
        query = query.Include(expression);
        return this;
    }
}

internal sealed class EntryIncludes<TEntity> : Expressions<TEntity> where TEntity : class
{
    private readonly EntityEntry<TEntity> entry;

    public EntryIncludes(EntityEntry<TEntity> entry)
    {
        this.entry = entry;
    }

    public override Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty?>> expression)
        where TProperty : class
    {
        entry.Reference(expression).Load();
        return this;
    }

    public override Expressions<TEntity> Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class
    {
        entry.Collection(expression).Load();
        return this;
    }
}