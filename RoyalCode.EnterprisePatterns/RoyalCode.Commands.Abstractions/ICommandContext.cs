namespace RoyalCode.Commands.Abstractions;

public interface ICommandContext<out TModel>
    where TModel : class
{
    TModel Model { get; }
}

public interface ICommandContext<out TRootEntity, out TModel>
    where TRootEntity : class
    where TModel : class
{
    TRootEntity Entity { get; }

    TModel Model { get; }
}

