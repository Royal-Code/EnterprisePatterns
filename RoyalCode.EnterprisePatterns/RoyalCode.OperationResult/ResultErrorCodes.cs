
namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Standard errors code.
/// </para>
/// </summary>
public static class ResultErrorCodes
{
    /// <summary>
    /// Errors where the parameters entered are invalid.
    /// </summary>
    public const string InvalidParameters = "400.1";

    /// <summary>
    /// Domain and entity state validation errors.
    /// </summary>
    public const string Validation = "400.2";

    /// <summary>
    /// Errors of some entity/registry not found.
    /// </summary>
    public const string NotFound = "404.1";

    /// <summary>
    /// User without access or permission.
    /// </summary>
    public const string Forbidden = "403.1";

    /// <summary>
    /// Application error, exception, which is not a validation error.
    /// </summary>
    public const string ApplicationError = "500.1";
}
