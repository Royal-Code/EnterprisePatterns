using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RoyalCode.OperationResult.ProblemDetails;

/// <summary>
/// A class that contains all the information about a problem details.
/// </summary>
public class ProblemDetailsDescriptor
{
    /// <summary>
    /// The key for the an aggregation of problems.
    /// </summary>
    public const string AggregateProblemsDetails = "aggregate-problems-details";

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
    ///     Try to get the descriptions of a problem details by its code.
    /// </para>
    /// </summary>
    /// <param name="code">The code of the problem details.</param>
    /// <param name="description">The descriptions of the problem details.</param>
    /// <returns>True if the descriptions was found, otherwise false.</returns>
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
    ///     Adds many problem details descriptions.
    /// </para>
    /// </summary>
    /// <param name="descriptions">A collection of descriptions of problem details.</param>
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
    ///     Adds many problem details descriptions.
    /// </para>
    /// </summary>
    /// <param name="json">A JSON string with the descriptions of problem details.</param>
    /// <returns>Same instance of <see cref="ProblemDetailsDescriptor"/>.</returns>
    /// <exception cref="ProblemDetailsDescriptorDeserializationException">
    ///     If the JSON string is invalid.
    /// </exception>
    public ProblemDetailsDescriptor AddFromJson(string json)
    {
        try
        {
            var descriptions = JsonSerializer.Deserialize<IEnumerable<ProblemDetailsDescription>>(json);
            if (descriptions is not null)
            {
                AddMany(descriptions);
            }
            return this;
        }
        catch(Exception ex)
        {
            throw new ProblemDetailsDescriptorDeserializationException(json, ex);
        }
    }
}
