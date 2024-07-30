using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Outbox.Abstractions.Contracts;

/// <summary>
/// Request to read messages from an outbox consumer.
/// </summary>
public sealed class GetMessages
{
    /// <summary>
    /// The consumer's name.
    /// </summary>
    public required string ConsumerName { get; init; }

    /// <summary>
    /// Number of messages to be read.
    /// </summary>
    public required int Limit { get; init; }

    /// <summary>
    /// Validates the properties of this object.
    /// </summary>
    /// <param name="problems">The problems encountered, if any.</param>
    /// <returns>
    ///     True if there are problems, i.e. the object has invalid properties, false if it is valid.
    /// </returns>
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<GetMessages>()
            .NotNull(ConsumerName)
            .MinMax(Limit, 1, 1000)
            .HasProblems(out problems);
    }
}
