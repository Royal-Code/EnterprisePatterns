
using Microsoft.AspNetCore.Mvc;
using RoyalCode.OperationResults.HttpResults;

namespace RoyalCode.OperationResults;

/// <summary>
/// Used by the <see cref="MatchErrorResult"/> 
/// to determine if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
/// </summary>
public static class ApiOperationResultOptions
{
    /// <summary>
    /// Set the default result type.
    /// </summary>
    public static void SetResultType(ApiResultTypes resultType)
    {
        switch (resultType)
        {
            case ApiResultTypes.AlwaysOperationResult:
                IsFlexible = false;
                IsProblemDetailsDefault = false;
                break;
            case ApiResultTypes.AlwaysProblemDetails:
                IsFlexible = false;
                IsProblemDetailsDefault = true;
                break;
            case ApiResultTypes.OperationResultAsDefault:
                IsFlexible = true;
                IsProblemDetailsDefault = false;
                break;
            case ApiResultTypes.ProblemDetailsAsDefault:
                IsFlexible = true;
                IsProblemDetailsDefault = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null);
        }
    }

    /// <summary>
    /// Determines if the API clients can decide the result type.
    /// </summary>
    public static bool IsFlexible { get; private set; } = false;

    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; private set; } = false;
}
