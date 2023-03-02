
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
    private static JsonSerializerOptions? jsonSerializerOptions;

    /// <inheritdoc />
    public string OrderBy { get; set; } = null!;

    /// <inheritdoc />
    public ListSortDirection Direction { get; set; }

    /// <summary>
    /// <para>
    ///     Try parse a string to a <see cref="Sorting"/>.
    /// </para>
    /// <para>
    /// <listheader>
    ///     Is accepted: 
    /// </listheader>
    /// <list type="bullet">
    /// <item>
    ///     The pattern <c>{PropertyName}</c>, <c>{PropertyName} asc</c>, <c>{PropertyName}-asc</c>,
    ///     <c>{PropertyName} desc</c> or <c>{PropertyName}-desc</c>.
    /// </item>
    /// <item>
    ///     A json string with the pattern <c>{"OrderBy": "{PropertyName}", "Direction": "{Direction}"}</c>.
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="orderBy">The string to be parsed.</param>
    /// <param name="sorting">The <see cref="Sorting"/> parsed.</param>
    /// <returns>
    ///     <c>true</c> if the string was parsed successfully; otherwise, <c>false</c>.
    /// </returns>
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
