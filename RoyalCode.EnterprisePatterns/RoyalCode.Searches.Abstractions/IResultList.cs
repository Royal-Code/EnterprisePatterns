
using System.ComponentModel;

namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     Interface de componentes para listagem do resultado de uma pesquisa.
/// </para>
/// <para>
///     Esta interface é uma abstração para o componente que contém os itens retornados da pesquisa:
///     <see cref="IResultList{TModel}"/>.
/// </para>
/// </summary>
public interface IResultList
{
    /// <summary>
    /// Número da página exibida.
    /// </summary>
    int Page { get; }

    /// <summary>
    /// Quantidade total de registros.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Quantidade de itens exibido por página.
    /// </summary>
    int ItemsPerPage { get; }

    /// <summary>
    /// Quantidade de páginas.
    /// </summary>
    int Pages { get; }

    /// <summary>
    /// The objects of ordination applied to search.
    /// </summary>
    IEnumerable<ISorting> Sortings { get; }

    /// <summary>
    /// Projeções executadas durante a pesquisa.
    /// </summary>
    Dictionary<string, object> Projections { get; }
}

/// <summary>
/// Interface de componentes para listagem do resultado de uma pesquisa.
/// </summary>
/// <typeparam name="TModel">Tipo de dado listado pelo resultado</typeparam>
public interface IResultList<TModel> : IResultList
{
    /// <summary>
    /// Lista dos dados da pesquisa.
    /// </summary>
    ICollection<TModel> Items { get; }

    /// <summary>
    /// Obtém um valor da projeção, caso ela exista e seja do tipo informado, 
    /// ou retorna o valor padrão, caso o valor não exista ou o tipo seja diferente.
    /// </summary>
    /// <typeparam name="T">Tipo do valor da projeção.</typeparam>
    /// <param name="name">Nome da projeção.</param>
    /// <param name="defaultValue">Valor padrão.</param>
    /// <returns>O valor da projeção, ou valor padrão.</returns>
    T GetProjection<T>(string name, T defaultValue = default);
}