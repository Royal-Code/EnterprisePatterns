using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extension class to get the header of the result type from the request context.
/// </summary>
public static class HeaderExtensions
{
    private const string OperationResultHeaderKey = "OperationResultHeader";
    private const string OperationResultHeaderDefaultValue = "X-Result";

    private static string? headerName;

    /// <summary>
    /// Try get the header of the result type from the request context.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <param name="resultType">The result type.</param>
    /// <returns>True if the header was found, otherwise false.</returns>
    public static bool TryGetResultTypeHeader(this HttpContext httpContext,
        [NotNullWhen(true)] out string? resultType)
    {
        if (headerName is null)
        {
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            headerName = configuration.GetValue<string>(OperationResultHeaderKey) ?? OperationResultHeaderDefaultValue;
        }

        if (httpContext.Request.Headers.TryGetValue(headerName, out var resultHeader))
        {
            resultType = resultHeader!;
            return true;
        }

        resultType = null;
        return false;
    }
}
