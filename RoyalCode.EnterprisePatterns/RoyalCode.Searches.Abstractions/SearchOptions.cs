
namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     A class that contains the options for a search.
///     It is used to define the search parameters.
/// </para>
/// <para>
///     It is design to retrieve the options from a query string and apply them to the search.
/// </para>
/// </summary>
public class SearchOptions
{
    /// <summary>
    /// <para>
    ///     Defines that the query will be paged and determines the number of items per page.
    /// </para>
    /// <para>
    ///     The default value is 10 items per page.
    /// </para>
    /// <para>
    ///     When zero (0) is entered, it will not be paged.
    /// </para>
    /// </summary>
    public int? ItemsPerPage { get; set; }

    /// <summary>
    /// The number of the page to be searched.
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// <para>
    ///     Updates the last record count.
    /// </para>
    /// <para>
    ///     Used to not count the records again.
    /// </para>
    /// </summary>
    public int? LastCount { get; set; }

    /// <summary>
    /// Whether to apply record counting.
    /// </summary>
    public bool? Count { get; set; }
}
