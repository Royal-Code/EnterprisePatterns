namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

/// <summary>
/// <para>
///     Exception of the <see cref="TransactionManager"/>.
/// </para>
/// <para>
///     Occurs when the DbContext is not correctly configured to use the unit of work.
/// </para>
/// </summary>
public class TransactionManagerInitializationException : Exception
{
    /// <summary>
    /// Create a new exception.
    /// </summary>
    public TransactionManagerInitializationException()
        : base("Interceptors not found")
    { }
}