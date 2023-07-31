
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace RoyalCode.OperationResults.Interceptors;

/// <summary>
/// Internal class to manage the interceptors for the <see cref="ResultErrors"/>.
/// </summary>
public static class MatchErrorInterceptors
{
    private static bool? hasInterceptors;

    /// <summary>
    /// Invoked when the <see cref="ResultErrors"/> is being executed.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="errors">The <see cref="ResultErrors"/> to be executed.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecutingError(HttpContext httpContext, ResultErrors errors)
    {
        IEnumerable<IMatchErrorInterceptor> interceptors;

        if (!hasInterceptors.HasValue)
            hasInterceptors = TryLoad(httpContext.RequestServices, out interceptors);
        else if (hasInterceptors.Value)
            interceptors = httpContext.RequestServices.GetRequiredService<IEnumerable<IMatchErrorInterceptor>>(); 
        else
            return;

        foreach (var interceptor in interceptors)
            try
            {
                interceptor.ExecutingError(errors);
            }
            catch (Exception ex)
            {
                var logger = httpContext.RequestServices.GetService<ILogger<IMatchErrorInterceptor>>()!;
                logger.LogError(ex, "Error executing the interceptor {Interceptor}", interceptor.GetType().FullName);
            }
    }

    private static bool? TryLoad(IServiceProvider requestServices, out IEnumerable<IMatchErrorInterceptor> interceptors)
    {
        interceptors = requestServices.GetService<IEnumerable<IMatchErrorInterceptor>>()
            ?? Enumerable.Empty<IMatchErrorInterceptor>();

        return interceptors.Any();
    }
}


/// <summary>
/// A interceptor for the <see cref="ResultErrors"/>.
/// </summary>
public interface IMatchErrorInterceptor
{
    /// <summary>
    /// Interceptor for the <see cref="ResultErrors"/> when is being executed as a result.
    /// </summary>
    /// <param name="errors"></param>
    void ExecutingError(ResultErrors errors);
}