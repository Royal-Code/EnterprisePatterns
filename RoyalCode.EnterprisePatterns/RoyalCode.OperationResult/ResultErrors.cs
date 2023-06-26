using System.Text.Json.Serialization;

namespace RoyalCode.OperationResults;

/// <summary>
/// A collection of <see cref="IResultMessage"/>s.
/// </summary>
public class ResultErrors : List<IResultMessage>, IOperationResult
{
    /// <summary>
    /// Default capacity of the collection.
    /// </summary>
    public static int DefaultCapacity { get; set; } = 1;

    /// <summary>
    /// Adds a new message to the collection and returns the collection.
    /// </summary>
    /// <param name="collection">The collection to add the message to</param>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    public static ResultErrors operator +(ResultErrors collection, IResultMessage message)
    {
        collection.Add(message);
        return collection;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ResultErrors"/>
    /// </summary>
    public ResultErrors() : base (DefaultCapacity) { }

    /// <summary>
    /// Creates a new instance of <see cref="ResultErrors"/> with the messages provided.
    /// </summary>
    /// <param name="messages">The collection of messages to add to the new instance.</param>
    [JsonConstructor]
    public ResultErrors(IEnumerable<ResultMessage> messages) : base(messages) { }

    /// <inheritdoc />
    public bool Success => false;

    /// <inheritdoc />
    public int ErrorsCount => Count;

    /// <inheritdoc />
    public IEnumerable<IResultMessage> Messages => this;

    /// <summary>
    /// Adds a new message to the collection and returns the collection.
    /// </summary>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    public ResultErrors With(IResultMessage message)
    {
        Add(message);
        return this;
    }

    /// <summary>
    /// Adds a range of messages to the collection and returns the collection.
    /// </summary>
    /// <param name="messages">The messages to add</param>
    /// <returns>The same instance of the collection</returns>
    public ResultErrors With(IEnumerable<IResultMessage> messages)
    {
        AddRange(messages);
        return this;
    }

    /// <summary>
    /// <para>
    ///     Returns a string representation of the collection.
    /// </para>
    /// <para>
    ///     The string will be a semicolon-separated list of the <see cref="IResultMessage"/> of each message in the collection.
    /// </para>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join("; ", this);
}
