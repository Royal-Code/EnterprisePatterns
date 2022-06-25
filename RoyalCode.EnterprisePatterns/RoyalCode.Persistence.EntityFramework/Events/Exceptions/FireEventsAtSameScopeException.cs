namespace RoyalCode.Persistence.EntityFramework.Events.Exceptions;

/// <summary>
/// Exception of the dispatch of events during the finalization of the unit of work.
/// </summary>
public class FireEventsAtSameScopeException : Exception
{
    /// <summary>
    /// Creates a new exception.
    /// </summary>
    public FireEventsAtSameScopeException(Exception innerException)
        : base(DomainEventResources.FireEventsAtSameScopeException, innerException)
    { }
}