using RoyalCode.Events.Outbox.Abstractions.Contracts;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Events.Outbox.Abstractions.Models;
using RoyalCode.Events.Outbox.Abstractions.Options;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RoyalCode.Events.Outbox.Abstractions.Services.Defaults;

/// <summary>
/// Abstract implementation for <see cref="IOutboxService"/>.
/// </summary>
public abstract class OutboxServiceBase : IOutboxService
{
    private readonly OutboxOptions options;
    private readonly IMessageDispatcher dispatcher;

    /// <summary>
    /// Creates a new instance of <see cref="OutboxServiceBase"/>.
    /// </summary>
    /// <param name="options">The outbox options.</param>
    /// <param name="dispatcher">The dispatcher of messagens.</param>
    protected OutboxServiceBase(
        IOptions<OutboxOptions> options,
        IMessageDispatcher dispatcher)
    {
        this.options = options.Value;
        this.dispatcher = dispatcher;
    }

    /// <summary>
    /// The handler to create new messages.
    /// </summary>
    protected abstract ICreateMessageHandler CreateMessageHandler { get; }

    /// <inheritdoc />
    public void Write(object message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageType = message.GetType();

        if (!options.Types.TryGetValue(messageType, out var metadata))
            throw new MessateTypeNotConfiguredException(messageType);

        var json = metadata.JsonTypeInfo is not null
            ? JsonSerializer.Serialize(metadata, metadata.JsonTypeInfo)
            : JsonSerializer.Serialize(message, metadata.SerializerOptions ?? options.SerializerOptions);

        var request = new CreateMessage()
        {
            MessageType = metadata.TypeName,
            VersionType = metadata.Version,
            Payload = json,
        };

        CreateMessageHandler.Handle(request).EnsureSuccess();
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IEnumerable<OutboxMessage> messages, CancellationToken ct)
    {
        foreach (var message in messages)
        {
            var metadata = options.Types.Values
                .FirstOrDefault(
                    x => x.TypeName == message.MessageType
                    && x.Version == message.VersionType)
                ?? throw new MessateTypeNotConfiguredException(message.MessageType);

            var payload = metadata.JsonTypeInfo is not null
                ? JsonSerializer.Deserialize(message.Payload, metadata.JsonTypeInfo)!
                : JsonSerializer.Deserialize(message.Payload, metadata.PayloadType, metadata.SerializerOptions ?? options.SerializerOptions)!;

            await dispatcher.DispatchAsync(payload, ct);
        }
    }
}
