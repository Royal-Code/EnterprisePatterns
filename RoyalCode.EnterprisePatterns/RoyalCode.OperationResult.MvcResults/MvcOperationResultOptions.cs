using Microsoft.AspNetCore.Mvc;

namespace RoyalCode.OperationResults;

/// <summary>
/// Options for used for <see cref="OperationMatchObjectResult"/> and <see cref="OperationMatchObjectResult{T}"/>.
/// </summary>
public static class MvcOperationResultOptions
{
    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; set; } = true;
}
