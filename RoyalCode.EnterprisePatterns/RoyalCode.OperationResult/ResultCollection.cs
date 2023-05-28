﻿
namespace RoyalCode.OperationResults;

/// <summary>
/// A collection of <see cref="IResultMessage"/>s.
/// </summary>
public class ResultCollection : List<IResultMessage>
{
    /// <summary>
    /// Creates a new instance of <see cref="ResultCollection"/>
    /// </summary>
    public ResultCollection() : base (1) { }

    /// <summary>
    /// Adds a new message to the collection and returns the collection.
    /// </summary>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    public ResultCollection With(IResultMessage message)
    {
        Add(message);
        return this;
    }

    /// <summary>
    /// Adds a range of messages to the collection and returns the collection.
    /// </summary>
    /// <param name="messages">The messages to add</param>
    /// <returns>The same instance of the collection</returns>
    public ResultCollection With(IEnumerable<IResultMessage> messages)
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