using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems;
using RoyalCode.WorkContext.Abstractions.Commands;

namespace RoyalCode.WorkContext.EntityFramework.Commands;

/// <summary>
/// Handles a command request using a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/> used for the operation.</typeparam>
/// <typeparam name="TCommand">The type of the command request to handle.</typeparam>
public interface ICommandHandler<TDbContext, TCommand>
    where TDbContext : DbContext
    where TCommand : ICommandRequest
{
    /// <summary>
    /// Handles the specified command request asynchronously.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="context">The work context containing the <typeparamref name="TDbContext"/>.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>A <see cref="Task{Result}"/> representing the asynchronous operation result.</returns>
    Task<Result> HandleAsync(
        TCommand request, IWorkContext<TDbContext> context, CancellationToken ct = default);
}

/// <summary>
/// Handles a command request using a specific <see cref="DbContext"/> and returns a response.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/> used for the operation.</typeparam>
/// <typeparam name="TCommand">The type of the command request to handle.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
public interface ICommandHandler<TDbContext, TCommand, TResponse>
    where TDbContext : DbContext
    where TCommand : ICommandRequest<TResponse>
{
    /// <summary>
    /// Handles the specified command request asynchronously and returns a response.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="context">The work context containing the <typeparamref name="TDbContext"/>.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation result with a response.</returns>
    Task<Result<TResponse>> HandleAsync(
        TCommand request, IWorkContext<TDbContext> context, CancellationToken ct = default);
}