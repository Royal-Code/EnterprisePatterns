using System.ComponentModel;

namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// Component that contains information about the sorting applied to a search/query.
/// </summary>
public interface ISorting
{
    /// <summary>
    /// Nome da ordenação a ser aplicada.
    /// </summary>
    string OrderBy { get; set; }

    /// <summary>
    /// Direção da ordenação:
    /// Ascending = 0, Descending = 1.
    /// </summary>
    ListSortDirection Direction { get; set; }
}
