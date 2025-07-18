using RoyalCode.SmartProblems;

namespace RoyalCode.WorkContext.Abstractions.Commands;

/// <summary>
/// Handles a command request using a <see cref="IWorkContext"/>
/// </summary>
/// <typeparam name="TCommand">The type of the command request to handle.</typeparam>
public interface ICommandHandler<TCommand>
    where TCommand : ICommandRequest
{
    /// <summary>
    /// Handles the specified command request asynchronously.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="context">The work context to access database.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>A <see cref="Task{Result}"/> representing the asynchronous operation result.</returns>
    Task<Result> HandleAsync(
        TCommand request, IWorkContext context, CancellationToken ct = default);
}

/// <summary>
/// Handles a command request using a <see cref="IWorkContext"/> and returns a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command request to handle.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommandRequest<TResponse>
{
    /// <summary>
    /// Handles the specified command request asynchronously and returns a response.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="context">The work context to access database.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation result with a response.</returns>
    Task<Result<TResponse>> HandleAsync(
        TCommand request, IWorkContext context, CancellationToken ct = default);
}