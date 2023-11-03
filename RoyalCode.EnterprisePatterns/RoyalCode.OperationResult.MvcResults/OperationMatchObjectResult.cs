using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoyalCode.OperationResults;

/// <summary>
/// A Mvc <see cref="ObjectResult"/> for <see cref="OperationResult"/>.
/// </summary>
public sealed class OperationMatchObjectResult : OperationMatchObjectResultBase<OperationResult>
{
    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; set; } = false;

    /// <summary>
    /// Creates a new instance of <see cref="OperationMatchObjectResult"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    /// <param name="formatPathWithValue">If true, the <paramref name="createdPath"/> will be formatted with the value of the result.</param>
    public OperationMatchObjectResult(
        OperationResult result,
        string? createdPath = null,
        bool formatPathWithValue = false) : base(result, createdPath, formatPathWithValue)
    { }

    /// <inheritdoc />
    protected override Task ExecuteMatchAsync(ActionContext context)
        => Result.Match(context, ExecuteSuccessResultAsync, ExecuteErrorResultAsync);

    private Task ExecuteSuccessResultAsync(ActionContext context)
    {
        Value = null;

        if (CreatedPath is not null)
        {
            StatusCode = StatusCodes.Status201Created;
            context.HttpContext.Response.Headers.Add("Location", CreatedPath);
        }
        else
        {
            StatusCode = StatusCodes.Status204NoContent;
        }
        
        return BaseExecuteResultAsync(context);
    }
}

/// <summary>
/// A MVC <see cref="ObjectResult"/> for <see cref="OperationResult{TValue}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public sealed class OperationMatchObjectResult<TValue> : OperationMatchObjectResultBase<OperationResult<TValue>>
{
    /// <summary>
    /// Creates a new instance of <see cref="OperationMatchObjectResult{TValue}"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    /// <param name="formatPathWithValue">If true, the <paramref name="createdPath"/> will be formatted with the value of the result.</param>
    public OperationMatchObjectResult(
        OperationResult<TValue> result,
        string? createdPath = null,
        bool formatPathWithValue = false) : base(result, createdPath, formatPathWithValue)
    { }

    /// <inheritdoc />
    protected override Task ExecuteMatchAsync(ActionContext context)
        => Result.Match(context, ExecuteSuccessResultAsync, ExecuteErrorResultAsync);

    private Task ExecuteSuccessResultAsync(TValue value, ActionContext context)
    {
        Value = value;

        if (CreatedPath is not null)
        {
            StatusCode = StatusCodes.Status201Created;

            var createdPath = FormatPathWithValue
                ? string.Format(CreatedPath, value)
                : CreatedPath;
            
            context.HttpContext.Response.Headers.Add("Location", createdPath);
        }
        else
        {
            StatusCode = StatusCodes.Status200OK;
        }

        return BaseExecuteResultAsync(context);
    }
}

/// <summary>
/// A Mvc <see cref="ObjectResult"/> for <see cref="OperationResult"/>.
/// </summary>
public sealed class ValidableMatchObjectResult : OperationMatchObjectResultBase<ValidableResult>
{
    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; set; } = false;

    /// <summary>
    /// Creates a new instance of <see cref="OperationMatchObjectResult"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    /// <param name="formatPathWithValue">If true, the <paramref name="createdPath"/> will be formatted with the value of the result.</param>
    public ValidableMatchObjectResult(
        ValidableResult result,
        string? createdPath = null,
        bool formatPathWithValue = false) : base(result, createdPath, formatPathWithValue)
    { }

    /// <inheritdoc />
    protected override Task ExecuteMatchAsync(ActionContext context)
        => Result.Match(context, ExecuteSuccessResultAsync, ExecuteErrorResultAsync);

    private Task ExecuteSuccessResultAsync(ActionContext context)
    {
        Value = null;

        if (CreatedPath is not null)
        {
            StatusCode = StatusCodes.Status201Created;
            context.HttpContext.Response.Headers.Add("Location", CreatedPath);
        }
        else
        {
            StatusCode = StatusCodes.Status204NoContent;
        }

        return BaseExecuteResultAsync(context);
    }
}