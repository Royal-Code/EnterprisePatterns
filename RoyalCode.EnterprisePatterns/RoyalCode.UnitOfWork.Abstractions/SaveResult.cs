using RoyalCode.OperationResult;

namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// <para>
///     Default implementation of <see cref="ISaveResult"/>.
/// </para>
/// </summary>
public class SaveResult : BaseResult, ISaveResult
{
    /// <inheritdoc />
    public int Changes { get; private set; }

    /// <summary>
    /// Success constructor
    /// </summary>
    /// <param name="changes">Number of entities modified.</param>
    public SaveResult(int changes) : base()
    {
        Changes = changes;
    }

    /// <inheritdoc />
    public SaveResult(Exception ex) : base(ResultMessage.Error(ex)) { }
}