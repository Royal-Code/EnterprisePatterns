using Microsoft.AspNetCore.Http.Metadata;

namespace RoyalCode.OperationResults.Metadata;

internal sealed class XResultHeaderMetadata : IFromHeaderMetadata
{
    internal static readonly XResultHeaderMetadata Instance = new();

    public string? Name => "X-Result";
}