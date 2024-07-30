using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Outbox.Abstractions.Models;
using RoyalCode.Outbox.Abstractions.Options;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RoyalCode.Outbox.Abstractions.Services.Defaults;

/// <summary>
/// Default implementation for <see cref="IOutboxService"/>.
/// </summary>
public sealed class OutboxService : IOutboxService
{
    private readonly OutboxOptions options;
    private readonly ICreateMessageHandler createMessageHandler;
    private readonly IMessageDispatcher dispatcher;

    /// <summary>
    /// Creates a new instance of <see cref="OutboxService"/>.
    /// </summary>
    /// <param name="options">The outbox options.</param>
    /// <param name="createMessageHandler">The handler to create new messages.</param>
    /// <param name="dispatcher">The dispatcher of messagens.</param>
    public OutboxService(
        IOptions<OutboxOptions> options,
        ICreateMessageHandler createMessageHandler,
        IMessageDispatcher dispatcher)
    {
        this.options = options.Value;
        this.createMessageHandler = createMessageHandler;
        this.dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public void Write(object message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageType = message.GetType();

        if (!options.Types.TryGetValue(messageType, out var metadata))
            throw new InvalidOperationException(
                $"O Tipo {messageType.Name} não foi configurado, é necessário configurar o tipo antes de escrever uma mensagem.");

        var settings = metadata.SerializerOptions ?? options.SerializerOptions;

        var json = JsonSerializer.Serialize(message, settings);

        var request = new CreateMessage()
        {
            MessageType = metadata.TypeName,
            VersionType = metadata.Version,
            Payload = json,
        };

        createMessageHandler.Handle(request).EnsureSuccess();
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
                ?? throw new InvalidOperationException($"O tipo {message.MessageType} não foi encontrado.");

            var settings = metadata.SerializerOptions ?? options.SerializerOptions;

            var payload = JsonSerializer.Deserialize(message.Payload, metadata.PayloadType, settings)!;

            await dispatcher.DispatchAsync(payload, ct);
        }
    }
}
