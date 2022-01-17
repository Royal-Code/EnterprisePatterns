
namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// <para>
///     A context for one Unit of Work.
/// </para>
/// <para>
///     This component must use other components to manage the entities loaded in memory during the unit of work.
/// </para>
/// <para>
///     The context of the unit of work must relate in some way to the repositories,
///     which are responsible for reading and writing entities.
/// </para>
/// <para>
///     At the end of a unit of work, the context will apply to the database (or storage) 
///     the changes made to entities during the unit of work (inclusion, modification, deletion).
/// </para>
/// <para>
///     For databases that support transactions, you can manage them by the context of the unit of work.
/// </para>
/// </summary>
public interface IUnitOfWorkContext
{
    /// <summary>
    /// <para>
    ///     Saves the changes in entities made by the services during the unit of work. 
    ///     Calling this method represents the end of the unit of work.
    /// </para>
    /// </summary>
    /// <returns>
    /// <para>
    ///     The save result.
    /// </para>
    /// </returns>
    ISaveResult Save();

    /// <summary>
    /// <para>
    ///     Saves the changes in entities made by the services during the unit of work. 
    ///     Calling this method represents the end of the unit of work.
    /// </para>
    /// </summary>
    /// <param name="token">Cancellation token for tasks.</param>
    /// <returns>
    /// <para>
    ///     The save result.
    /// </para>
    /// </returns>
    Task<ISaveResult> SaveAsync(CancellationToken token = default);

    /// <summary>
    /// <para>
    ///     Starts a transaction and returns a component to handle it.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This command is not normally required for the unit of work.
    /// </para>
    /// <para>
    ///     The Design Pattern Unit of Work is responsible for collecting
    ///     the changes in the entities during the unit of work,
    ///     initiating a transaction and sending the commands to the database,
    ///     and finalizing the transaction.
    /// </para>
    /// <para>
    ///     There are cases where a single transaction does not solve the unit of work 
    ///     and it is necessary to perform the operation in parts. 
    ///     The manual use of transactions is justified when it is necessary 
    ///     to send data to the database several times during the same transaction.
    /// </para>
    /// <para>
    ///     Other operations, such as registry lock (not recommended), can be done through transactions.
    /// </para>
    /// </remarks>
    /// <returns>
    /// <para>
    ///     Object to manipulate the transaction.
    /// </para>
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// <para>
    ///     If the persistence technology does not overcome transactions.
    /// </para>
    /// </exception>
    ITransaction BeginTransaction();

    /// <summary>
    /// <para>
    ///     Starts a transaction and returns a component to handle it.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This command is not normally required for the unit of work.
    /// </para>
    /// <para>
    ///     The Design Pattern Unit of Work is responsible for collecting
    ///     the changes in the entities during the unit of work,
    ///     initiating a transaction and sending the commands to the database,
    ///     and finalizing the transaction.
    /// </para>
    /// <para>
    ///     There are cases where a single transaction does not solve the unit of work 
    ///     and it is necessary to perform the operation in parts. 
    ///     The manual use of transactions is justified when it is necessary 
    ///     to send data to the database several times during the same transaction.
    /// </para>
    /// <para>
    ///     Other operations, such as registry lock (not recommended), can be done through transactions.
    /// </para>
    /// </remarks>
    /// <returns>
    /// <para>
    ///     Object to manipulate the transaction.
    /// </para>
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// <para>
    ///     If the persistence technology does not overcome transactions.
    /// </para>
    /// </exception>
    Task<ITransaction> BeginTransactionAsync(CancellationToken token = default);

    /// <summary>
    /// <para>
    ///     Clears context by removing entities from tracking, 
    ///     or similar component that keeps entities in memory.
    /// </para>
    /// </summary>
    /// <param name="force">
    /// <para>
    ///     Whether all entities should be removed, or only the unmodified ones.
    /// </para>
    /// <para>
    ///     True removes all entities, false only unmodified ones.
    /// </para>
    /// </param>
    void CleanUp(bool force = true);
}
