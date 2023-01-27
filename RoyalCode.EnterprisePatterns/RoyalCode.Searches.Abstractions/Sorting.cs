
using System.ComponentModel;
using System.Text.Json;

namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     Default implementation of <see cref="ISorting"/>.
/// </para>
/// </summary>
public class Sorting : ISorting
{
    public string? OrderBy { get; set; }

    public ListSortDirection Direction { get; set; }

    private static JsonSerializerOptions? jsonSerializerOptions;
    public static bool TryParse(string? orderBy, out Sorting sorting)
    {
        if (orderBy is null)
        {
            sorting = default!;
            return false;
        }

        // check if orderBy param is in json format
        if (orderBy.Length > 2 && orderBy[0] == '{' && orderBy[^1] == '}')
        {
            jsonSerializerOptions ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var jsonSorting = JsonSerializer.Deserialize<Sorting>(orderBy, jsonSerializerOptions);
            if (string.IsNullOrWhiteSpace(jsonSorting?.OrderBy))
            {
                sorting = default!;
                return false;
            }
            sorting = jsonSorting;
            return true;
        }


        // check if the param orderBy ends with asc or desc in a case insensitive way
        var isAscending = orderBy.EndsWith("asc", StringComparison.OrdinalIgnoreCase);
        var isDescending = orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase);

        // remove the asc or desc from the orderBy
        orderBy = isAscending
            ? orderBy[..^3]
            : isDescending
                ? orderBy[..^4]
                : orderBy;

        sorting = new Sorting
        {
            OrderBy = isAscending
                ? orderBy.Substring(0, orderBy.Length - 3)
                : isDescending
                    ? orderBy.Substring(0, orderBy.Length - 4)
                    : orderBy,
            Direction = isDescending ? ListSortDirection.Descending : ListSortDirection.Ascending
        };
        return true;
    }
}
