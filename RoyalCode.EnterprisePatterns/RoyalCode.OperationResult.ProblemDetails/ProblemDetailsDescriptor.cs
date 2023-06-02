using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RoyalCode.OperationResults;

/// <summary>
/// A class that contains all the information about a problem details.
/// </summary>
public class ProblemDetailsDescriptor
{
    /// <summary>
    /// The key for the an aggregation of problems.
    /// </summary>
    public const string AggregateProblemsDetails = "aggregate-problems-details";

    /// <summary>
    /// Extension field for the problems details of an aggregation of problems.
    /// </summary>
    public const string AggregateExtensionField = "inner-details";

    /// <summary>
    /// Extension field for the details of invalid parameters.
    /// </summary>
    public const string InvalidParametersExtensionField = "invalid-params";

    /// <summary>
    /// Extension field for internal server errors messagens
    /// </summary>
    public const string ErrorsExtensionField = "errors";

    /// <summary>
    /// Extension field for the details of an entity not found.
    /// </summary>
    public const string NotFoundExtensionField = "not-found";

    /// <summary>
    /// The Detail field for the an aggregation of problems.
    /// </summary>
    public static string AggregateMessage { get; set; } = "Multiples problems";

    /// <summary>
    /// The Detail field for the an internal error.
    /// </summary>
    public static string InternalErrorsMessage { get; set; } = "Internal error";

    /// <summary>
    /// The Detail field for the an invalid parameters.
    /// </summary>
    public static string InvalidParametersMessage { get; set; } = "Invalid parameters";

    /// <summary>
    /// The Detail field for the an entity not found.
    /// </summary>
    public static string NotFoundMessage { get; set; } = "Entity not found";

    /// <summary>
    /// The Detail field for the an undefined error.
    /// </summary>
    public static string DefaultMessage { get; set; } = "An error has occurred";

    private readonly Dictionary<string, ProblemDetailsDescription> descriptions = new()
    {
        {
            GenericErrorCodes.NotFound,
            new ProblemDetailsDescription(GenericErrorCodes.NotFound,
                "https://www.rfc-editor.org/rfc/rfc9110.html#name-404-not-found",
                "Entity not found",
                """
                The 404 (Not Found) status code indicates that the origin server did not find a current representation
                for the target resource or is not willing to disclose that one exists. 
                """)
        },
        {
            GenericErrorCodes.InvalidParameters,
            new ProblemDetailsDescription(GenericErrorCodes.InvalidParameters,
                "https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request",
                "The input parameters are invalid",
                """
                The 400 (Bad Request) status code indicates that the server cannot or will not process the request
                due to something that is perceived to be a client error.
                This particular error occurs because the parameters sent in by the client are invalid.
                """
                    )
        },
        {
            GenericErrorCodes.Validation,
            new ProblemDetailsDescription(GenericErrorCodes.Validation,
                "https://www.rfc-editor.org/rfc/rfc9110.html#name-422-unprocessable-content",
                "Errors have occurred in the validation of the input parameters.",
                """
                The 422 (Unprocessable Content) status code indicates that the server understands the content type 
                of the request content, and the syntax of the request content is correct, 
                but it was unable to process the contained instructions.
                This occurs because of validations on the input parameters,
                where application or business rules are violated.
                """)
        },
        {
            GenericErrorCodes.ApplicationError,
            new ProblemDetailsDescription(GenericErrorCodes.ApplicationError,
                "https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error",
                "Internal server error",
                """
                The 500 (Internal Server Error) status code indicates that the server 
                encountered an unexpected condition that prevented it from fulfilling the request.
                """)
        },
        {
            AggregateProblemsDetails,
            new ProblemDetailsDescription(AggregateProblemsDetails,
                "Multiples problems",
                """
                This type of problem describes that there were several problems, and they are of different types. 
                An additional property, called 'inner-problems' will contain the various problems.
                """)
        }
    };

    /// <summary>
    /// <para>
    ///     Try to get the descriptionsToAdd of a problem details by its code.
    /// </para>
    /// </summary>
    /// <param name="code">The code of the problem details.</param>
    /// <param name="description">The descriptionsToAdd of the problem details.</param>
    /// <returns>True if the descriptionsToAdd was found, otherwise false.</returns>
    public bool TryGetDescription(string code, [NotNullWhen(true)] out ProblemDetailsDescription? description)
    {
        return descriptions.TryGetValue(code, out description);
    }

    /// <summary>
    /// <para>
    ///     Adds a new problem details description.
    /// </para>
    /// </summary>
    /// <param name="description">The description of the problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    public ProblemDetailsDescriptor Add(ProblemDetailsDescription description)
    {
        descriptions.Add(description.Code, description);
        return this;
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd.
    /// </para>
    /// </summary>
    /// <param name="descriptions">A collection of descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    public ProblemDetailsDescriptor AddMany(IEnumerable<ProblemDetailsDescription> descriptions)
    {
        foreach (var description in descriptions)
        {
            this.descriptions.Add(description.Code, description);
        }

        return this;
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd from a JSON string.
    /// </para>
    /// </summary>
    /// <param name="json">A JSON string with the descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    /// <exception cref="ProblemDetailsDescriptorDeserializationException">
    ///     If the JSON string is invalid.
    /// </exception>
    public ProblemDetailsDescriptor AddFromJson(string json)
    {
        try
        {
            var descriptionsToAdd = JsonSerializer.Deserialize<IEnumerable<ProblemDetailsDescription>>(json);
            if (descriptionsToAdd is not null)
            {
                AddMany(descriptionsToAdd);
            }
            return this;
        }
        catch(Exception ex)
        {
            throw new ProblemDetailsDescriptorDeserializationException(json, ex);
        }
    }

    /// <summary>
    /// <para>
    ///     Adds many problem details descriptionsToAdd from a JSON file.
    /// </para>
    /// </summary>
    /// <param name="path">The path of the JSON file with the descriptionsToAdd of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    /// <exception cref="ProblemDetailsDescriptorDeserializationException">
    ///     If the JSON file is invalid.
    /// </exception>
    public ProblemDetailsDescriptor AddFromJsonFile(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            return AddFromJson(json);
        }
        catch (ProblemDetailsDescriptorDeserializationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ProblemDetailsDescriptorDeserializationException(string.Empty, ex);
        }
    }
}
