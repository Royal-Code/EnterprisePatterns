﻿using Microsoft.Extensions.Logging;
using RoyalCode.OperationResults;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ProblemDetailsServiceCollectionExtensions
{
    /// <summary>
    /// <para>
    ///     Add the <see cref="ProblemDetailsOptions"/> to the <see cref="IServiceCollection"/>.
    /// </para>
    /// <para>
    ///     Bind the configuration section <c>ProblemDetails</c> to the <see cref="ProblemDetailsOptions"/>.
    /// </para>
    /// <para>
    ///     Execute the <see cref="ProblemDetailsOptions.Complete(ILogger)"/> when the configuration is completed.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>Same instance of <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddProblemDetailsDescriptions(this IServiceCollection services)
    {
        services.AddOptions<ProblemDetailsOptions>()
            .BindConfiguration("ProblemDetails")
            .PostConfigure<ILogger<ProblemDetailsOptions>>((o, l) =>
            {
                // log completing the options configuration
                l.LogDebug("Completing the options configuration.");

                // complete the options configuration
                o.Complete(l);
            });

        return services;
    }
}
