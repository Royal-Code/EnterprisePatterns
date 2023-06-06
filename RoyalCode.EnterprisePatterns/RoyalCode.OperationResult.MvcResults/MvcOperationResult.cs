using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResults.Convertion;

namespace RoyalCode.OperationResults;

/// <summary>
/// MVC <see cref="ObjectResult"/> for <see cref="IOperationResult"/>.
/// </summary>
public class MvcOperationResult : ObjectResult
{
    /// <summary>
    /// Creates a new instance of <see cref="MvcOperationResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path created by the operation.</param>
    /// <param name="formatPathWithValue">
    ///     If true, the <paramref name="createdPath"/> will be formatted with the value of the result.
    /// </param>
    public MvcOperationResult(IOperationResult result, string? createdPath, bool formatPathWithValue) 
        : base(result)
    {
        Result = result;
        CreatedPath = createdPath;
        FormatPathWithValue = formatPathWithValue;
    }

    /// <summary>
    /// The <see cref="IOperationResult"/>.
    /// </summary>
    public IOperationResult Result { get; }

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
        var httpContext = context.HttpContext;

        if (httpContext.TryGetResultTypeHeader(out var resultType) && resultType == "ProblemDetails")
            CreateProblemDetailsResult(httpContext);
        else
            CreateOperationResult(httpContext);

        return base.ExecuteResultAsync(context);
    }

    private void CreateProblemDetailsResult(HttpContext httpContext)
    {
        if (Result.Success)
        {
            CreateDefaultSuccessResult(httpContext);
            return;
        }

        var options = httpContext.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        var problemDetails = Result.ToProblemDetails(options);

        Value = problemDetails;
        ContentTypes.Add("application/problem+json");
        StatusCode = problemDetails.Status;
        DeclaredType = typeof(ProblemDetails);
    }

    private void CreateOperationResult(HttpContext httpContext)
    {
        if (Result.Success)
        {
            CreateDefaultSuccessResult(httpContext);
        }
        else
        {
            CreateDefaultFailureResult();
        }
    }

    private void CreateDefaultSuccessResult(HttpContext httpContext)
    {
        IResultHasValue? valueResult = Result as IResultHasValue;
        bool hasValue = valueResult?.Value is not null;

        if (httpContext.Request.Method == "GET")
        {
            Value = hasValue
                ? valueResult!.Value
                : valueResult;
            ContentTypes.Add("application/json");
            StatusCode = 200;

            return ;
        }

        if (CreatedPath is not null)
        {
            var createdPath = CreatedPath;
            if (FormatPathWithValue && hasValue)
                createdPath = string.Format(createdPath, valueResult!.Value);

            Value = valueResult?.Value;
            ContentTypes.Add("application/json");
            StatusCode = 201;
            httpContext.Response.Headers.Add("Location", createdPath);

            return;
        }

        if (!hasValue)
        {
            Value = null;
            StatusCode = 204;
            return;
        }

        Value = valueResult!.Value;
        ContentTypes.Add("application/json");
        StatusCode = Result.GetHttpStatus();
    }

    private void CreateDefaultFailureResult()
    {
        Value = Result;
        ContentTypes.Add("application/json");
        StatusCode = Result.GetHttpStatus();
        DeclaredType = Result.GetType();
    }
}
