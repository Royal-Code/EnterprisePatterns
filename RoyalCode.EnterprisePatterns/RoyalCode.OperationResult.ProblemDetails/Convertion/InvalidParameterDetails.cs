using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.ProblemDetails.Convertion;

public class InvalidParameterDetails
{
    public InvalidParameterDetails(string reason)
    {
        Reason = reason;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    public string Reason { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object>? Extensions { get; set; }
}
