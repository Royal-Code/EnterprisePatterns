namespace RoyalCode.WorkContext.Abstractions.Commands;

/// <summary>
/// Represents a request to execute a command by some command handler.
/// </summary>
/// <remarks>
///     This interface serves as a marker for command request objects, which are typically used in 
///     command-query separation (CQRS) patterns or similar architectures.
///     <br />    
///     Implementations of this interface define the specific data and behavior required to execute a command.
/// </remarks>
public interface ICommandRequest { }

/// <summary>
/// Represents a request that can be executed to produce a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response produced by the request.</typeparam>
public interface ICommandRequest<TResponse> { }