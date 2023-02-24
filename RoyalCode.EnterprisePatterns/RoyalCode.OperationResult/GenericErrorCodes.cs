
namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Generic errors code.
/// </para>
/// <para>
///     These are generic error types,
///     where it is not necessary to specify details about the error that occurred and the rules involved.
/// </para>
/// <para>
///     In cases of specific errors, specific and unique codes must be used for each type of error.
/// </para>
/// </summary>
public static class GenericErrorCodes
{
    /// <summary>
    /// <para>
    ///     Errors where the parameters entered are invalid.
    /// </para>
    /// <para>
    ///     It should be used on syntax errors, where the service cannot "understand" the input values.
    /// </para>
    /// </summary>
    public const string InvalidParameters = "400";

    /// <summary>
    /// <para>
    ///     Errors where the input parameters are not valid by some application rule or domain.
    /// </para>
    /// <para>
    ///     It should be used on semantic errors,
    ///     where the request is understood but the dataset is not valid by some rule.
    /// </para>
    /// </summary>
    public const string Validation = "422";

    /// <summary>
    /// Errors of some entity/registry not found.
    /// </summary>
    public const string NotFound = "404";

    /// <summary>
    /// Application error, exception, which is not a validation error.
    /// </summary>
    public const string ApplicationError = "500";
}
