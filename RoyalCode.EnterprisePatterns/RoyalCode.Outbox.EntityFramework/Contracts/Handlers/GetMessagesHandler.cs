using Microsoft.EntityFrameworkCore;
using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Contracts.Responses;
using RoyalCode.Outbox.Abstractions.Models;
using RoyalCode.SmartProblems;

namespace RoyalCode.Outbox.EntityFramework.Contracts.Handlers;

/// <inheritdoc />
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public class GetMessagesHandler<TDbContext> : IGetMessagesHandler
    where TDbContext : DbContext
{
    private readonly TDbContext db;

    /// <summary>
    /// Creates a new instance of <see cref="GetMessagesHandler{TDbContext}"/>.
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/>.</param>
    public GetMessagesHandler(TDbContext db)
    {
        this.db = db;
    }

    /// <inheritdoc />
    public async Task<Result<RetrievedMessages>> HandleAsync(GetMessages request, CancellationToken ct)
    {
        if (request.HasProblems(out var problems))
            return problems;

        var consumer = await db.Set<OutboxConsumer>()
            .FirstOrDefaultAsync(c => c.Name == request.ConsumerName, ct);

        if (consumer == null)
            return Problems.InvalidParameter(
                R.ConsumerNotFound,
                nameof(GetMessages.ConsumerName));

        var messages = await db.Set<OutboxMessage>()
            .AsNoTracking()
            .Where(m => m.Id > consumer.LastConsumedMessageId)
            .OrderBy(m => m.Id)
            .Take(request.Limit + 1)
            .ToListAsync(ct);

        var hasMore = messages.Count > request.Limit;
        if (hasMore)
            messages = messages.Take(request.Limit).ToList();

        return new RetrievedMessages()
        {
            Messages = messages,
            Count = messages.Count,
            HasMore = hasMore
        };
    }
}
