namespace RoyalCode.OperationResult.ProblemDetails.Convertion;

public class ProblemDetailsBuilder
{
    private List<InvalidParameterDetails>? invalidParameterErrors;
    private List<NotFoundDetails>? notFoundErrors;
    private List<string>? internalErrors;
    private List<IResultMessage>? customProblems;
    private Dictionary<string, object>? extensions;

    public string? Code { get; private set; }

    public bool Aggregate { get; private set; }

    public bool GenericError { get; private set; }

    public int Status { get; private set; } = 0;

    public void SetCode(string code, bool isGenericError)
    {
        if (Aggregate is true)
            return;

        if (Code is null)
        {
            Code = code;
            GenericError = isGenericError;
            return;
        }

        if (isGenericError is false)
        {
            if (GenericError is false)
            {
                Aggregate = true;
                Code = ProblemDetailsDescriptor.AggregateProblemsDetails;
            }
            else
            {
                Code = code;
                GenericError = false;
            }
        }
        else if (GenericError is true && GenericErrorCodes.HaveMorePriority(Code, code))
        {
            Code = code;
        }
    }

    public void AddInvalidParameter(InvalidParameterDetails details)
    {
        invalidParameterErrors ??= new List<InvalidParameterDetails>();
        invalidParameterErrors.Add(details);
    }

    public void AddNotFound(NotFoundDetails notFoundDetails)
    {
        notFoundErrors ??= new List<NotFoundDetails>();
        notFoundErrors.Add(notFoundDetails);
    }

    public void AddInternalErrorMessage(string message)
    {
        internalErrors ??= new List<string>();
        internalErrors.Add(message);
    }

    public void AddCustomProblem(IResultMessage message)
    {
        customProblems ??= new List<IResultMessage>();
        customProblems.Add(message);
    }

    public void AddExtension(IEnumerable<KeyValuePair<string, object>> additionalInformation)
    {
        extensions ??= new Dictionary<string, object>();

        foreach (var kvp in additionalInformation)
        {
            extensions[kvp.Key] = kvp.Value;
        }
    }
}
