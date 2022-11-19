
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

internal class StagedSaveChangesInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        var transactionManager = eventData.Context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.None)
        {
            transactionManager.SavingStarted(eventData.Context);
        }
        else if (transactionManager.Stage == TransactionStage.FirstStage)
        {
            transactionManager.SecondStageStarted(eventData.Context);
        }

        return result;
    }
    
    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context is null)
            return result;

        var transactionManager = eventData.Context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.FirstStage
            && transactionManager.WillSaveChangesInTwoStages)
        {
            // second stage of save changes.
            result += eventData.Context.SaveChanges();
        }
        else if (transactionManager.Stage == TransactionStage.SecondStage)
        {
            
        }

        return result;
    }

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        throw new NotImplementedException();
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
