
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
    public string OrderBy { get; set; }

    public ListSortDirection Direction { get; set; }

    private static JsonSerializerOptions? jsonSerializerOptions;
    public static bool TryParse(string? orderBy, out Sorting sorting)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
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
        // when is ascending, the order by ends with ' asc' or '-asc'
        // when is descending, the order by ends with ' desc' or '-desc'
        var isAscending = orderBy.EndsWith(" asc", StringComparison.OrdinalIgnoreCase)
            || orderBy.EndsWith("-asc", StringComparison.OrdinalIgnoreCase);
        var isDescending = orderBy.EndsWith(" desc", StringComparison.OrdinalIgnoreCase)
            || orderBy.EndsWith("-desc", StringComparison.OrdinalIgnoreCase);

        // remove the asc or desc from the orderBy
        orderBy = isAscending
            ? orderBy[..^4]
            : isDescending
                ? orderBy[..^5]
                : orderBy;

        sorting = new Sorting
        {
            OrderBy = orderBy,
            Direction = isDescending ? ListSortDirection.Descending : ListSortDirection.Ascending
        };
        return true;
    }
}
