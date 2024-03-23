namespace RoyalCode.UnitOfWork.EntityFramework.Exceptions;

/// <summary>
/// <para>
///     Exception of the <see cref="UnitOfWork{TDbContext}"/>.
/// </para>
/// <para>
///     Occurs when the DbContext is not correctly configured to use the unit of work.
/// </para>
/// </summary>
public class UnitOfWorkInitializationException : Exception
{
    /// <summary>
    /// Create a new exception.
    /// </summary>
    public UnitOfWorkInitializationException()
        : base(UnitOfWorkResources.InvalidaInitialization)
    { }
}