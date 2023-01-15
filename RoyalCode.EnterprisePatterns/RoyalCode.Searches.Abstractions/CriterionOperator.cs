namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     The operator used to filter.
/// </para>
/// </summary>
public enum CriterionOperator
{
    /// <summary>
    ///     The criterion is equal to the value.
    /// </summary>
    Equal,
    /// <summary>
    ///     The criterion is not equal to the value.
    /// </summary>
    NotEqual,
    /// <summary>
    ///     The criterion is greater than the value.
    /// </summary>
    GreaterThan,
    /// <summary>
    ///     The criterion is greater than or equal to the value.
    /// </summary>
    GreaterThanOrEqual,
    /// <summary>
    ///     The criterion is less than the value.
    /// </summary>
    LessThan,
    /// <summary>
    ///     The criterion is less than or equal to the value.
    /// </summary>
    LessThanOrEqual,
    
    ///// <summary>
    /////     The criterion is in the values.
    ///// </summary>
    //In,
    ///// <summary>
    /////     The criterion is not in the values.
    ///// </summary>
    //NotIn,
    
    /// <summary>
    ///     The criterion is like the value.
    /// </summary>
    Like,
    /// <summary>
    ///     The criterion is not like the value.
    /// </summary>
    NotLike,
    /// <summary>
    ///     The criterion is null.
    /// </summary>
    IsNull,
    /// <summary>
    ///     The criterion is not null.
    /// </summary>
    IsNotNull,
}