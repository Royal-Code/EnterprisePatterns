using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Outbox.Abstractions.Contracts.Responses;
using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.Outbox.Api;

/// <summary>
/// Api for Outbox reading operations.
/// </summary>
public static class OutboxApi
{
    /// <summary>
    /// Adds/Maps the endpoints for reading the Outbox via HTTP.
    /// </summary>
    /// <param name="group"></param>
    public static void MapOutbox(this RouteGroupBuilder group)
    {
        var outboxGroup = group.MapGroup("outbox")
            .WithName("outbox")
            .WithDescription("Outbox API");

        outboxGroup.MapPost("consumer", RegisterConsumerAsync)
            .WithName("register-consumer")
            .WithDescription("Register a new outbox consumer")
            .WithOpenApi();

        outboxGroup.MapGet("consumer/{consumer}/messages", GetConsumerMessagesAsync)
            .WithName("get-outbox-messages")
            .WithDescription("get the outbox next messages for the consumer")
            .WithOpenApi();

        outboxGroup.MapPost("consumer/{consumer}/commit", CommitConsumedAsync)
            .WithName("commit-consumed-outbox-messages")
            .WithDescription("Commit the consumed messages")
            .WithOpenApi();
    }

    private static async Task<OkMatch> CommitConsumedAsync(
        string consumer, [FromBody] long lastConsumedId, ICommitConsumedHandler handler, CancellationToken ct)
    {
        var request = new CommitConsumed()
        {
            ConsumerName = consumer,
            LastConsumedMessageId = lastConsumedId,
        };

        return await handler.HandleAsync(request, ct);
    }

    private static async Task<OkMatch<RetrievedMessages>> GetConsumerMessagesAsync(
        string consumer, [FromQuery] int limit, IGetMessagesHandler handler, CancellationToken ct)
    {
        var request = new GetMessages()
        {
            ConsumerName = consumer,
            Limit = limit > 0 ? limit : 10
        };

        return await handler.HandleAsync(request, ct);
    }

    private static async Task<OkMatch> RegisterConsumerAsync(
        RegisterConsumer request, IRegisterConsumerHandler handler, CancellationToken ct)
    {
        return await handler.HandleAsync(request, ct);
    }
}
