namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Enumerator of the message types of an operation result.
/// </para>
/// </summary>
public enum ResultMessageType
{
    /// <summary>
    /// Type of error message, failure, invalidations.
    /// </summary>
    Error = 0,

    /// <summary>
    /// Type of alert, warning message.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Type of information message.
    /// </summary>
    Info = 2,

    /// <summary>
    /// Type of success message.
    /// </summary>
    Success = 3,
}
