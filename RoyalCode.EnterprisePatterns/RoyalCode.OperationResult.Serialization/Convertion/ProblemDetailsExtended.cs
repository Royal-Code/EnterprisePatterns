using RoyalCode.OperationResults.Convertion.Internals;
using System.Net;
using System.Text.Json;
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
    private const string AggregateExtensionField = "inner-details";
    private const string InvalidParametersExtensionField = "invalid-params";
    private const string ErrorsExtensionField = "errors";
    private const string NotFoundExtensionField = "not-found";
    private const string GenericErrorTitle = "An error has occurred";
    private const string NotFoundTitle = "Entity not found";
    private const string InvalidParametersTitle = "The input parameters are invalid";
    private const string ValidationTitle = "Errors have occurred in the validation of the input parameters.";
    private const string ApplicationErrorTitle = "An error has occurred";

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Invalid parameters errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(InvalidParametersExtensionField)]
    public IEnumerable<InvalidParameterDetails>? InvalidParameters { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Not found errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(NotFoundExtensionField)]
    public IEnumerable<NotFoundDetails>? NotFoundDetails { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Internal errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(ErrorsExtensionField)]
    public IEnumerable<string>? Errors { get; set; }

    /// <summary>
    /// <para>
    ///     Inner problem details. When a problem details is wrapped by another problem details,
    ///     the inner problem details are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(AggregateExtensionField)]
    public IEnumerable<ProblemDetails>? InnerProblemDetails { get; set; }

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
            // if status is 400 then use invalid parameters, otherwise use validation
            var useValidation = Status != 400;

            foreach (var invalidParameter in InvalidParameters)
            {
                var message = useValidation
                    ? ResultMessage.ValidationError(invalidParameter.Reason, invalidParameter.Name ?? string.Empty)
                    : ResultMessage.InvalidParameter(invalidParameter.Reason, invalidParameter.Name ?? string.Empty);
                if (invalidParameter.Extensions is not null)
                    foreach(var extension in invalidParameter.Extensions)
                        message.WithAdditionInfo(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                erros += message;
            }

            if (Title == InvalidParametersTitle || Title == ValidationTitle)
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
                        message.WithAdditionInfo(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                erros += message;
            }

            if (Title == NotFoundTitle)
                ignoreDetails = true;
        }

        if (Errors is not null)
        {
            bool isInternalError = Status == 500;

            foreach (var error in Errors)
            {
                erros += isInternalError
                    ? ResultMessage.ApplicationError(error)
                    : ResultMessage.Error(GenericErrorCodes.GenericError, error, HttpStatusCode.BadRequest);
            }

            if (Title == ApplicationErrorTitle || Title == GenericErrorTitle)
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

        if (ignoreDetails)
        {
            if (Extensions?.Count > 0)
            {
                var message = (ResultMessage)erros[0];
                foreach (var extension in Extensions)
                    message.WithAdditionInfo(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);
            }
        }
        else
        {
            erros += ToResultMessage(this);
        }

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
                message.WithAdditionInfo(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

        return message;
    }

    private static object? ReadJsonValue(object? obj)
    {
        if (obj is null)
            return null;

        if (obj is JsonElement jsonElement)
            return ReadJsonElement(jsonElement);

        return obj;
    }

    private static object? ReadJsonElement(JsonElement jsonElement)
    {
        // convert the json element to a object of the correct type
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => jsonElement.EnumerateArray().Select(ReadJsonElement).ToArray(),
            JsonValueKind.Object => ReadJsonObject(jsonElement),
            _ => jsonElement.GetRawText(),
        };
    }

    private static object ReadJsonObject(JsonElement jsonElement)
    {
        // convert the json element to a object of the correct type
        var obj = new Dictionary<string, object?>();
        foreach (var property in jsonElement.EnumerateObject())
        {
            obj.Add(property.Name, ReadJsonElement(property.Value));
        }

        return obj;
    }
}
