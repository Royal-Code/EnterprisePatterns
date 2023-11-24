using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Hints;

internal sealed class HintHandlerBuilder<THint> : IHintHandlerBuilder<THint>
    where THint : Enum
{
    private readonly IHintHandlerRegistry registry;

    internal HintHandlerBuilder(IHintHandlerRegistry registry)
    {
        this.registry = registry;
    }

    public IHintHandlerBuilder<THint> Add<TEntity>(Action<THint, Includes<TEntity>> action)
        where TEntity : class
    {
        registry.AddIncludesHandler(action);
        return this;
    }
}
