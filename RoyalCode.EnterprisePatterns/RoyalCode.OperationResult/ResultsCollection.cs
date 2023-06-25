using System.Text.Json.Serialization;

namespace RoyalCode.OperationResults;

// TODO: Rename to ResultErrors

/// <summary>
/// A collection of <see cref="IResultMessage"/>s.
/// </summary>
public class ResultsCollection : List<IResultMessage>, IOperationResult
{
    /// <summary>
    /// Adds a new message to the collection and returns the collection.
    /// </summary>
    /// <param name="collection">The collection to add the message to</param>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    public static ResultsCollection operator +(ResultsCollection collection, IResultMessage message)
    {
        collection.Add(message);
        return collection;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ResultsCollection"/>
    /// </summary>
    public ResultsCollection() : base (1) { }

    [JsonConstructor]
    public ResultsCollection(IEnumerable<ResultMessage> messages) : base(messages) { }

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
    public ResultsCollection With(IResultMessage message)
    {
        Add(message);
        return this;
    }

    /// <summary>
    /// Adds a range of messages to the collection and returns the collection.
    /// </summary>
    /// <param name="messages">The messages to add</param>
    /// <returns>The same instance of the collection</returns>
    public ResultsCollection With(IEnumerable<IResultMessage> messages)
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