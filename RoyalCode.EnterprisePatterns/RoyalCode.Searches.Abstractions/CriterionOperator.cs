namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     The operator used to filter.
/// </para>
/// </summary>
public enum CriterionOperator
{
    /// <summary>
    ///     Automatic choise the operator.
    /// </summary>
    Auto,
    
    /// <summary>
    ///     The criterion is equal to the value.
    /// </summary>
    Equal,
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
    
    /// <summary>
    ///     The criterion is like the value.
    /// </summary>
    Like,
    
    ///// <summary>
    /////     The criterion is null.
    ///// </summary>
    //IsNull,
}