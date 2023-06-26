using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResults.Convertion;

/// <summary>
/// <para>
///     A Problem Details with extensions. See <see cref="ProblemDetails"/> for more information.
/// </para>
/// <para>
///     Additional properties are added to this class to support the extensions, and they are:
/// </para>
/// <list type="bullet">
///     <item>
///         <term><see cref="InvalidParameters"/></term> that contains the invalid parameters details, 
///         like the parameter name, the error message (reason) and additional information.
///     </item>
///     <item>
///         <term><see cref="NotFoundDetails"/></term> that contains the not found details,
///         like the property name and the error message adn additional information.
///     </item>
/// </list>
/// </summary>
public class ProblemDetailsExtended : ProblemDetails
{
    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Invalid parameters errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(ProblemDetailsDescriptor.InvalidParametersExtensionField)]
    public IEnumerable<InvalidParameterDetails>? InvalidParameters { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Not found errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(ProblemDetailsDescriptor.NotFoundExtensionField)]
    public IEnumerable<NotFoundDetails>? NotFoundDetails { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Internal errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(ProblemDetailsDescriptor.ErrorsExtensionField)]
    public IEnumerable<string>? InternalErrors { get; set; }

    /// <summary>
    /// <para>
    ///     Inner problem details. When a problem details is wrapped by another problem details,
    ///     the inner problem details are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(ProblemDetailsDescriptor.AggregateExtensionField)]
    public IEnumerable<ProblemDetails>? InnerProblemDetails { get; set; }

    /// <summary>
    /// <para>
    ///     Determines if the problem details is an aggregate of other problem details.
    /// </para>
    /// <para>
    ///     When is an aggregate, see <see cref="InnerProblemDetails"/>.
    /// </para>
    /// <para>
    ///     The problem details is an aggregate when the <see cref="ProblemDetails.Type"/>
    ///     is <see cref="ProblemDetailsDescriptor.AggregateProblemsDetails"/>.
    /// </para>
    /// </summary>
    public bool IsAggregate => Type == ProblemDetailsDescriptor.AggregateProblemsDetails;

    /// <summary>
    /// <para>
    ///     Convert the <see cref="ProblemDetailsExtended"/> to a <see cref="ResultErrors"/>.
    /// </para>
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="ResultErrors"/> with the <see cref="ResultMessage"/> created from the <see cref="ProblemDetailsExtended"/>.
    /// </returns>
    public ResultErrors ToResultErrors()
    {
        var erros = new ResultErrors();

        bool ignoreDetails = false;

        if (InvalidParameters is not null)
        {
            foreach (var invalidParameter in InvalidParameters)
            {
                var message = ResultMessage.InvalidParameters(invalidParameter.Reason, invalidParameter.Name ?? string.Empty);
                if (invalidParameter.Extensions is not null)
                    foreach(var extension in invalidParameter.Extensions)
                        message.WithAdditionInfo(extension.Key, extension.Value);

                erros += message;
            }

            if (Title == ProblemDetailsDescriptor.Defaults.InvalidParametersTitle
                || Title == ProblemDetailsDescriptor.Defaults.ValidationTitle)
            {
                ignoreDetails = true;
            }
        }

        if (NotFoundDetails is not null)
        {
            foreach (var notFoundDetail in NotFoundDetails)
            {
                var message = ResultMessage.NotFound(notFoundDetail.Message, notFoundDetail.Property ?? string.Empty);
                if (notFoundDetail.Extensions is not null)
                    foreach(var extension in notFoundDetail.Extensions)
                        message.WithAdditionInfo(extension.Key, extension.Value);

                erros += message;
            }

            if (Title == ProblemDetailsDescriptor.Defaults.NotFoundTitle)
                ignoreDetails = true;
        }

        if (InternalErrors is not null)
        {
            foreach (var internalError in InternalErrors)
            {
                erros += ResultMessage.ApplicationError(internalError);
            }

            if (Title == ProblemDetailsDescriptor.Defaults.ApplicationErrorTitle)
                ignoreDetails = true;
        }

        if (InnerProblemDetails is not null)
        {
            foreach (var innerProblemDetail in InnerProblemDetails)
            {
                erros += ToResultMessage(innerProblemDetail);
            }

            ignoreDetails = true;
        }

        if (!ignoreDetails)
            erros += ToResultMessage(this);

        return erros;
    }

    private static ResultMessage ToResultMessage(ProblemDetails details)
    {
        // creates the result message
        var message = ResultMessage.Error(
            code: details.Type ?? GenericErrorCodes.ApplicationError, 
            text: details.Detail ?? GenericErrorCodes.ApplicationError,
            status: details.Status.HasValue ? (HttpStatusCode)details.Status.Value : null,
            ex: null);
        
        // add the additional information
        if (details.Extensions is not null)
            foreach(var extension in details.Extensions)
                message.WithAdditionInfo(extension.Key, extension.Value ?? string.Empty);

        return message;
    }
}
