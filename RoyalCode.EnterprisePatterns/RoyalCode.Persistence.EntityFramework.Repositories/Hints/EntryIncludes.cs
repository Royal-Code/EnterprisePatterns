using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Hints;

internal sealed class EntryIncludes<TEntity> : Includes<TEntity> where TEntity : class
{
    private readonly EntityEntry<TEntity> entry;

    public EntryIncludes(EntityEntry<TEntity> entry)
    {
        this.entry = entry;
    }

    public override Includes<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty?>> expression)
        where TProperty : class
    {
        entry.Reference(expression).Load();
        return this;
    }

    public override Includes<TEntity> Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class
    {
        entry.Collection(expression).Load();
        return this;
    }
}
