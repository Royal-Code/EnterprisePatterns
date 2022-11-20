namespace RoyalCode.EntityFramework.StagedSaveChanges;

/// <summary>
/// <para>
///     Enum that represents the state of the transaction.
/// </para>
/// </summary>
public enum TransactionStage
{
    /// <summary>
    /// The first stage of the transaction.
    /// </summary>
    FirstStage,

    /// <summary>
    /// The second stage of the transaction.
    /// </summary>
    SecondStage,

    /// <summary>
    /// The transaction is not being used.
    /// </summary>
    None
}