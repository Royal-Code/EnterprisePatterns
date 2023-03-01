using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.ProblemDetails.Convertion;

public class NotFoundDetails
{
    public NotFoundDetails(string message)
    {
        Message = message;
    }

    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Property { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object>? Extensions { get; set; }
}
