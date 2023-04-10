namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// <para>
///     The context that contains the data of the request and other data required for create a new entity.
/// </para>
/// </summary>
/// <typeparam name="TRequest">The type of the request data.</typeparam>
public interface ICreationContext<TRequest>
{
    /// <summary>
    /// The request data for create a new entity.
    /// </summary>
    TRequest RequestData { get; }
}

/// <summary>
/// <para>
///     The context that contains the data of the request. the root entity of the creation 
///     and other data required for create a new entity.
/// </para>
/// </summary>
/// <typeparam name="TRequest">The type of the request data.</typeparam>
/// <typeparam name="TRootEntity">The type of the root entity.</typeparam>
public interface ICreationContext<TRequest, TRootEntity>
{
    /// <summary>
    /// The aggregate root entity.
    /// </summary>
    TRootEntity RootEntity { get; }

    /// <summary>
    /// The request data for create a new entity.
    /// </summary>
    TRequest RequestData { get; }
}

