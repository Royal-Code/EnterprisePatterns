using Microsoft.EntityFrameworkCore;
using RoyalCode.Events.Outbox.Abstractions.Contracts;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Events.Outbox.Abstractions.Models;
using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.EntityFramework.Services.Handlers;

/// <inheritdoc />
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public sealed class CommitConsumedHandler<TDbContext> : ICommitConsumedHandler
    where TDbContext : DbContext
{
    private readonly TDbContext db;

    /// <summary>
    /// Creates a new instance of <see cref="CommitConsumedHandler{TDbContext}"/>.
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/>.</param>
    public CommitConsumedHandler(TDbContext db)
    {
        this.db = db;
    }

    /// <inheritdoc />
    public async Task<Result> HandleAsync(CommitConsumed request, CancellationToken ct)
    {
        var consumer = await db.Set<OutboxConsumer>()
            .FirstOrDefaultAsync(c => c.Name == request.ConsumerName, ct);

        if (consumer == null)
            return Problems.InvalidParameter(R.ConsumerNotFound)
                .With("consumer_name", request.ConsumerName);

        consumer.LastConsumedMessageId = request.LastConsumedMessageId;
        await db.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
