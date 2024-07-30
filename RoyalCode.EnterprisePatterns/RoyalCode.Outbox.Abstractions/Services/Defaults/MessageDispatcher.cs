using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.Outbox.Abstractions.Services.Defaults;

/// <summary>
/// <para>
///     Default implementation for <see cref="IMessageDispatcher"/>.
/// </para>
/// <para>
///     For each dispatch, a new service scope is created and the messagens dispatched via <see cref="MessageDispatcher{TMessage}"/>.
/// </para>
/// </summary>
public sealed class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Creates a new instance of <see cref="MessageDispatcher"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider to create the services scopes and <see cref="MessageDispatcher{TMessage}"/></param>
    public MessageDispatcher(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Task DispatchAsync(object message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        var dispatcherType = typeof(MessageDispatcher<>).MakeGenericType(message.GetType());

        using var scope = serviceProvider.CreateScope();
        var dispatcher = (IMessageDispatcher)scope.ServiceProvider.GetRequiredService(dispatcherType);
        return dispatcher.DispatchAsync(message, ct);
    }
}

/// <summary>
/// A service that dispatch a specific type of message.
/// </summary>
/// <typeparam name="TMessage">The type of the message to be dispatched.</typeparam>
public sealed class MessageDispatcher<TMessage> : IMessageDispatcher
{
    private readonly IEnumerable<IMessageObserver<TMessage>> observers;

    /// <summary>
    /// Creates a new instance of <see cref="MessageDispatcher{TMessage}"/>.
    /// </summary>
    /// <param name="observers">The collection of observer for the <typeparamref name="TMessage"/>.</param>
    public MessageDispatcher(IEnumerable<IMessageObserver<TMessage>> observers)
    {
        this.observers = observers;
    }

    /// <summary>
    /// Dispatch messages of type <typeparamref name="TMessage"/>.
    /// If can't cast, a exception will be throwed.
    /// </summary>
    /// <param name="message">The message, must be <typeparamref name="TMessage"/>.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    public async Task DispatchAsync(object message, CancellationToken ct)
    {
        TMessage messageTyped = (TMessage)message;
        foreach (var observer in observers)
        {
            await observer.HandleAsync(messageTyped, ct);
        }
    }
}
