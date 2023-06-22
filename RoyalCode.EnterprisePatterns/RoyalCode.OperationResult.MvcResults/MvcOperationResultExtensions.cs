
using Microsoft.AspNetCore.Http;
using RoyalCode.OperationResults;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="ObjectResult"/>.
/// </summary>
public static class MvcResults
{
    /// <summary>
    /// Convert <see cref="IOperationResult"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path for created responses, when applyable.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static ObjectResult ToResult(this IOperationResult result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return new MvcOperationResult(result, createdPath, formatPathWithValue);
    }

    /// <summary>
    /// Convert a <see cref="OperationResult"/> to an <see cref="OperationMatchObjectResult"/>
    /// to be used in a MVC controller as a return type.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    /// <returns>The <see cref="OperationMatchObjectResult"/> for the response.</returns>
    public static OperationMatchObjectResult ToResult(this OperationResult result)
        => new(result);

    /// <summary>
    /// Convert a <see cref="OperationResult"/> to an <see cref="OperationMatchObjectResult"/>
    /// to be used in a MVC controller as a return type.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="OperationMatchObjectResult"/> for the response.</returns>
    public static OperationMatchObjectResult ToResult(this OperationResult result,
        string createdPath, bool formatPathWithValue = false)
        => new(result, createdPath, formatPathWithValue);

    /// <summary>
    /// Convert a <see cref="OperationResult{TValue}"/> to an <see cref="OperationMatchObjectResult{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="OperationResult{TValue}"/> to be converted.</param>
    /// <returns>The <see cref="OperationMatchObjectResult{TValue}"/> for the response.</returns>
    public static OperationMatchObjectResult<TValue> ToResult<TValue>(this OperationResult<TValue> result)
        => new(result);

    /// <summary>
    /// Convert a <see cref="OperationResult{TValue}"/> to an <see cref="OperationMatchObjectResult{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="OperationResult{TValue}"/> to be converted.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="OperationMatchObjectResult{TValue}"/> for the response.</returns>
    public static OperationMatchObjectResult<TValue> ToResult<TValue>(this OperationResult<TValue> result,
        string createdPath, bool formatPathWithValue = false)
        => new(result, createdPath, formatPathWithValue);

    /// <summary>
    /// Convert a <see cref="ValidableResult"/> to an <see cref="ValidableMatchObjectResult"/>
    /// to be used in a MVC controller as a return type.
    /// </summary>
    /// <param name="result">The <see cref="ValidableResult"/> to be converted.</param>
    /// <returns>The <see cref="ValidableMatchObjectResult"/> for the response.</returns>
    public static ValidableMatchObjectResult ToResult(this ValidableResult result)
        => new(result);
}