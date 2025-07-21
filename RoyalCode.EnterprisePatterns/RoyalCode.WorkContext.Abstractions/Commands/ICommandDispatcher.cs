using RoyalCode.SmartProblems;

namespace RoyalCode.WorkContext.Commands;

/// <summary>
/// Defines a contract for dispatching command requests.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Sends a command request to the appropriate command handler for execution.
    /// </summary>
    /// <param name="request">The command request to be executed.</param>
    /// <param name="ct">A CancellationToken.</param>
    /// <returns>The result of the command execution.</returns>
    Task<Result> SendAsync(ICommandRequest request, CancellationToken ct = default);

    /// <summary>
    /// Sends a command request that produces a response of type <typeparamref name="TResponse"/> 
    /// to the appropriate command handler for execution.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request">The command request to be executed.</param>
    /// <param name="ct">A CancellationToken.</param>
    /// <returns>The result of the command execution.</returns>
    Task<Result<TResponse>> SendAsync<TResponse>(ICommandRequest<TResponse> request, CancellationToken ct = default);
}
