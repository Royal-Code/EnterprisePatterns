using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RoyalCode.Events.Outbox.Abstractions.Contracts;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Responses;
using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.Events.Outbox.Api;

/// <summary>
/// Api for Outbox reading operations.
/// </summary>
public static class OutboxApi
{
    /// <summary>
    /// Adds/Maps the endpoints for reading the Outbox via HTTP.
    /// </summary>
    /// <param name="app">Application endpoint router.</param>
    /// <param name="groupPrefix">Optional, group prefix, default is 'outbox'.</param>
    public static void MapOutbox(this IEndpointRouteBuilder app, string groupPrefix = "outbox")
    {
        var outboxGroup = app.MapGroup(groupPrefix)
            .WithName("outbox")
            .WithDescription("Outbox API");

        outboxGroup.MapOutbox();
    }

    /// <summary>
    /// Adds/Maps the endpoints for reading the Outbox via HTTP.
    /// </summary>
    /// <param name="group">The RouteGroup.</param>
    public static void MapOutbox(this RouteGroupBuilder group)
    {
        group.MapPost("consumer", RegisterConsumerAsync)
            .WithName("register-consumer")
            .WithDescription("Register a new outbox consumer")
            .WithOpenApi();

        group.MapGet("consumer/{consumer}/messages", GetConsumerMessagesAsync)
            .WithName("get-outbox-messages")
            .WithDescription("get the outbox next messages for the consumer")
            .WithOpenApi();

        group.MapPost("consumer/{consumer}/commit", CommitConsumedAsync)
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
