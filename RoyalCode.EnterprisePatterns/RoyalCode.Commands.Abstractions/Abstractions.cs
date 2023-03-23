
namespace RoyalCode.Commands.Abstractions;

public interface ICommandContextFactory
{
    Task<TContext> CreateAsync<TContext, TModel>(TModel model)
        where TContext : ICommandContext<TModel>
        where TModel : class;

    Task<TContext> CreateAsync<TContext, TRootEntity, TModel>(TRootEntity entity, TModel model)
        where TContext : ICommandContext<TRootEntity, TModel>
        where TRootEntity : class
        where TModel : class;
}

