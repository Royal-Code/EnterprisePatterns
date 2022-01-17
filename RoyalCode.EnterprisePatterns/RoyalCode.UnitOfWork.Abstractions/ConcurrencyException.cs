namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// Exception for when a database concurrency failure occurs.
/// </summary>
public class ConcurrencyException : Exception
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">Original Exception.</param>
    public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }
}
