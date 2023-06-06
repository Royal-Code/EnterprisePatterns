
using Microsoft.AspNetCore.Mvc;

namespace RoyalCode.OperationResults;

/// <summary>
/// Used by the <see cref="MatchErrorResult"/> 
/// to determine if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
/// </summary>
public static class ApiOperationResultOptions
{
    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; set; } = false;
}
