﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResults.Convertion;

namespace RoyalCode.OperationResults;

/// <summary>
/// Abstract MVC <see cref="ObjectResult"/> for <see cref="OperationResult"/> and <see cref="OperationResult{TValue}"/>.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public abstract class OperationMatchObjectResultBase<TResult> : ObjectResult
{
    /// <summary>
    /// Creates a new instance of <see cref="OperationMatchObjectResultBase{TResult}"/>.
    /// </summary>
    /// <param name="result">The result.
    /// <param name="createdPath">Optinal, the path created by the operation.</param>
    /// <param name="formatPathWithValue">If true, the <paramref name="createdPath"/> will be formatted with the value of the result.</param>
    protected OperationMatchObjectResultBase(TResult result,
        string? createdPath = null,
        bool formatPathWithValue = false)
        : base(result)
    {
        Result = result;
        CreatedPath = createdPath;
        FormatPathWithValue = formatPathWithValue;
    }

    /// <summary>
    /// The <see cref="OperationResult"/> or <see cref="OperationResult{TValue}"/>
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// The path created by the operation.
    /// </summary>
    public string? CreatedPath { get; }

    /// <summary>
    /// If true, the <see cref="CreatedPath"/> will be formatted with the value of the result.
    /// </summary>
    public bool FormatPathWithValue { get; }

    /// <inheritdoc />
    public override Task ExecuteResultAsync(ActionContext context)
    {
        return ExecuteMatchAsync(context);
    }

    /// <summary>
    /// Protected method to execute the result.
    /// </summary>
    /// <param name="context">The <see cref="ActionContext"/>.</param>
    /// <returns>A <see cref="Task"/> that will complete when the result is executed.</returns>
    protected Task BaseExecuteResultAsync(ActionContext context)
        => base.ExecuteResultAsync(context);

    /// <inheritdoc />
    protected abstract Task ExecuteMatchAsync(ActionContext context);

    /// <summary>
    /// Executes the result for the given <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The result error.</param>
    /// <param name="context">The <see cref="ActionContext"/>.</param>
    /// <returns>A <see cref="Task"/> that will complete when the result is executed.</returns>
    protected Task ExecuteErrorResultAsync(ResultsCollection error, ActionContext context)
    {
        context.HttpContext.TryGetResultTypeHeader(out var resultType);
        return resultType switch
        {
            "ProblemDetails" => ExecuteProblemDetailsAsync(error, context),
            "OperationResult" => ExecuteOperationResultAsync(error, context),
            _ => ExecuteDefaultErrorAsync(error, context)
        };
    }

    private Task ExecuteDefaultErrorAsync(ResultsCollection error, ActionContext context)
        => MvcOperationResultOptions.IsProblemDetailsDefault
            ? ExecuteProblemDetailsAsync(error, context)
            : ExecuteOperationResultAsync(error, context);

    private Task ExecuteProblemDetailsAsync(ResultsCollection error, ActionContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        var problemDetails = error.ToProblemDetails(options);

        Value = problemDetails;
        ContentTypes.Add("application/problem+json");
        StatusCode = problemDetails.Status;
        DeclaredType = typeof(ProblemDetails);

        return base.ExecuteResultAsync(context);
    }

    private Task ExecuteOperationResultAsync(ResultsCollection error, ActionContext context)
    {
        Value = error;
        ContentTypes.Add("application/json");
        StatusCode = error.GetHttpStatus();
        DeclaredType = typeof(IOperationResult);

        return base.ExecuteResultAsync(context);
    }
}
