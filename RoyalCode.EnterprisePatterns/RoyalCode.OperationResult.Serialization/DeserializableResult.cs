﻿
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.Serialization;

/// <summary>
/// A result that can be deserialized.
/// </summary>
public class DeserializableResult : IOperationResult
{
    /// <summary>
    /// Determine whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore] 
    public bool Success => ErrorsCount == 0;

    /// <summary>
    /// The message of the result.
    /// </summary>
    public List<ResultMessage>? Messages { get; set; }

    /// <inheritdoc />
    [JsonIgnore] 
    public int ErrorsCount => Messages?.Count ?? 0;

    /// <inheritdoc />
    IEnumerable<IResultMessage> IOperationResult.Messages => Messages ?? Enumerable.Empty<IResultMessage>();

    /// <summary>
    /// Convert the <see cref="DeserializableResult"/> to <see cref="DeserializableResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A new instance of <see cref="DeserializableResult{TValue}"/>.</returns>
    public DeserializableResult<TValue> WithValue<TValue>(TValue value) => new(value, this);
}

/// <summary>
/// A result that can be deserialized.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class DeserializableResult<TValue> : DeserializableResult, IOperationResult<TValue>
{
    /// <summary>
    /// Default constructor for deserialize.
    /// </summary>
    public DeserializableResult() { }

    /// <summary>
    /// Used by <see cref="DeserializableResult.WithValue{TValue}(TValue)"/>.
    /// </summary>
    internal DeserializableResult(TValue? value, DeserializableResult publicResult)
    {
        Value = value;
        Messages = publicResult.Messages;
    }

    /// <summary>
    /// The operation result value.
    /// </summary>
    public TValue? Value { get; set; }

    [JsonIgnore]
    object? IResultHasValue.Value => Value;
}