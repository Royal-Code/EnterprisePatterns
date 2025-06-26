using Microsoft.EntityFrameworkCore;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.SmartSearch.EntityFramework.Services;
using RoyalCode.SmartSearch.Linq.Services;
using RoyalCode.SmartSearch.Linq.Sortings;

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal class WorkContextCriteriaPerformer<TEntity> : CriteriaPerformerBase<TEntity>
    where TEntity : class
{
    private readonly DbContext db;
    private readonly IHintPerformer? hintPerformer;

    public WorkContextCriteriaPerformer(
        DbContext db,
        IHintPerformer? hintPerformer,
        ISpecifierFactory specifierFactory, 
        IOrderByProvider orderByProvider, 
        ISelectorFactory selectorFactory) 
        : base(specifierFactory, orderByProvider, selectorFactory)
    {
        this.db = db;
        this.hintPerformer = hintPerformer;
    }

    protected override IQueryable<TEntity> GetQueryable(bool trackingEnabled)
    {
        var query = trackingEnabled
            ? db.Set<TEntity>()
            : db.Set<TEntity>().AsNoTracking();

        if (hintPerformer is not null)
            query = hintPerformer.Perform(query);

        return query;
    }
}
