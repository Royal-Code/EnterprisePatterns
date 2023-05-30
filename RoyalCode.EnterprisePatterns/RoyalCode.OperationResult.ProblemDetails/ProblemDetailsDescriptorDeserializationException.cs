﻿namespace RoyalCode.OperationResults;

/// <summary>
/// Exception thrown when a collection of <see cref="ProblemDetailsDescription"/> cannot be deserialized.
/// </summary>
public sealed class ProblemDetailsDescriptorDeserializationException : Exception
{
    private const string MessagePattern = 
        "Failed to deserialize a collection of ProblemDetailsDescription, JSON:\n{0}";

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="json">The JSON string used to deserialization.</param>
    /// <param name="innerException">The original exception.</param>
    public ProblemDetailsDescriptorDeserializationException(string json, Exception? innerException)
        : base(string.Format(MessagePattern, json), innerException)
    { }
}