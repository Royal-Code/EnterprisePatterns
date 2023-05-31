
using RoyalCode.OperationResults;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="IResult"/>.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    /// Convert <see cref="IOperationResult"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path for created responses, when applyable.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult(this IOperationResult result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return new ApiOperationResult(result, createdPath, formatPathWithValue);
    }
}
