using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResult.ProblemDetails.Convertion;

namespace RoyalCode.OperationResult.MvcResults;

public class MvcOperationResult : ObjectResult
{
    private const string OperationResultHeaderKey = "OperationResultHeader";
    private const string OperationResultHeaderDefaultValue = "X-Result";

    private static string? headerName;

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

    public override Task ExecuteResultAsync(ActionContext context)
    {
        var httpContext = context.HttpContext;

        if (headerName is null)
        {
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            headerName = configuration.GetValue<string>(OperationResultHeaderKey) ?? OperationResultHeaderDefaultValue;
        }

        if (httpContext.TryGetResultTypeHeader(out var resultType))
        {
            if (resultType == "ProblemDetails")
                CreateProblemDetailsResult(httpContext);
            if (resultType == "OperationResult")
                CreateOperationResult();
        }
        else if (Result.Success)
        {
            CreateDefaultSuccessResult(httpContext);
        }
        else
        {
            CreateDefaultFailureResult();
        }

        return base.ExecuteResultAsync(context);
    }

    private void CreateProblemDetailsResult(HttpContext httpContext)
    {
        if (Result.Success)
        {
            CreateDefaultSuccessResult(httpContext);
            return;
        }

        var options = httpContext.RequestServices.GetRequiredService<IOptions<ProblemDetails.ProblemDetailsOptions>>().Value;
        var problemDetails = Result.ToProblemDetails(options);

        Value = problemDetails;
        ContentTypes.Add("application/problem+json");
        StatusCode = problemDetails.Status;
        DeclaredType = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails);
    }

    private void CreateOperationResult()
    {
        Value = Result;
        ContentTypes.Add("application/json");
        StatusCode = Result.GetHttpStatus();
        DeclaredType = Result.GetType();
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
