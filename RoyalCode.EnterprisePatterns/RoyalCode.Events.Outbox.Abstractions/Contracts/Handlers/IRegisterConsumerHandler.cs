using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;

/// <summary>
/// Handler to register a new Outbox consumer.
/// </summary>
public interface IRegisterConsumerHandler
{
    /// <summary>
    /// Register a new consumer. Consumers must have unique names.
    /// If the consumer already exists, the operation will fail.
    /// </summary>
    /// <param name="request">Request to register a new Outbox consumer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<Result> HandleAsync(RegisterConsumer request, CancellationToken ct);

    /// <summary>
    /// Checks if the consumer can be registered, or if a consumer already exists for the name entered.
    /// </summary>
    /// <param name="request">Request to register a new Outbox consumer.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    ///     True if the consumer can be registered, 
    ///     false if a consumer already exists for the name entered and cannot be registered.
    /// </returns>
    Task<bool> CanRegisterAsync(RegisterConsumer request, CancellationToken ct);
}
