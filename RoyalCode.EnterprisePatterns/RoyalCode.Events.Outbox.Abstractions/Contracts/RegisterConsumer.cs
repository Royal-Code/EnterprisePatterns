using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Events.Outbox.Abstractions.Contracts;

/// <summary>
/// Request to register a consumer for the outbox.
/// </summary>
public sealed class RegisterConsumer
{
    /// <summary>
    /// Name of consumer.
    /// </summary>
    public required string ConsumerName { get; init; }

    /// <summary>
    /// Whether to consume messages from the last one.
    /// </summary>
    public bool ConsumeFromLastMessage { get; init; }

    /// <summary>
    /// Apply validations to the data model.
    /// </summary>
    /// <param name="problems">Problems if the model is invalid.</param>
    /// <returns>True if there are problems, i.e. it is invalid, false if it is valid.</returns>
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<RegisterConsumer>()
            .NotEmpty(ConsumerName)
            .Length(ConsumerName, 3, 100)
            .HasProblems(out problems);
    }
}
