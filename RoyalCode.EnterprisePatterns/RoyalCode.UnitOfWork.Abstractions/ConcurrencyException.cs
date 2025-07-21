namespace RoyalCode.UnitOfWork;

/// <summary>
/// Exception for when a database concurrency failure occurs.
/// </summary>
/// <remarks>
/// Default constructor
/// </remarks>
/// <param name="message">The exception message.</param>
/// <param name="innerException">Original Exception.</param>
public class ConcurrencyException(string message, Exception innerException) 
    : Exception(message, innerException)
{ }
