namespace RoyalCode.Commands.Abstractions;

public interface ICreationHandler<out TEntity, in TModel>
    where TEntity : class
    where TModel : class
{
    TEntity Create(TModel model);
}

public interface ICreationHandler<out TEntity, in TContext, TModel>
    where TEntity : class
    where TModel : class
    where TContext : ICommandContext<TModel>
{
    TEntity Create(TContext context);
}

public interface ICreationHandler<TRootEntity, out TEntity, in TContext, TModel>
    where TRootEntity : class
    where TEntity : class
    where TModel : class
    where TContext : ICommandContext<TRootEntity, TModel>
{
    TEntity Create(TContext context);
}