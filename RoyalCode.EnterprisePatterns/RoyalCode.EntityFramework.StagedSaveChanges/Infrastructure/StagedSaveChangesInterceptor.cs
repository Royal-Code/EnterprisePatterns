﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

/// <summary>
/// <para>
///     Internal interceptor of Entity Framework.
/// </para>
/// <para>
///     This interceptor is used to intercept the save changes of the <see cref="DbContext"/>.
/// </para>
/// <para>
///     When required, this class will call the <see cref="ITransactionManager"/> to manage the transaction,
///     and start a second stage of the save changes.
/// </para>
/// </summary>
internal class StagedSaveChangesInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        var transactionManager = context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.None)
        {
            transactionManager.Saving();
        }
        else if (transactionManager.Stage == TransactionStage.FirstStage)
        {
            transactionManager.Staged();
        }

        return result;
    }
    
    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        var transactionManager = context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.FirstStage
            && transactionManager.WillSaveChangesInTwoStages)
        {
            // second stage of save changes.
            result += context.SaveChanges();
        }
        else
        {
            transactionManager.Saved(result);
        }
        
        return result;
    }

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        var context = eventData.Context;
        if (context is null)
            return;

        var transactionManager = context.GetService<ITransactionManager>();

        transactionManager.Failed();
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        var transactionManager = context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.None)
        {
            await transactionManager.SavingAsync(cancellationToken);
        }
        else if (transactionManager.Stage == TransactionStage.FirstStage)
        {
            await transactionManager.StagedAsync(cancellationToken);
        }

        return result;
    }

    public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        var transactionManager = context.GetService<ITransactionManager>();

        if (transactionManager.Stage == TransactionStage.FirstStage
            && transactionManager.WillSaveChangesInTwoStages)
        {
            // second stage of save changes.
            result += await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await transactionManager.SavedAsync(result, cancellationToken);
        }
        
        return result;
    }

    public async Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return;

        var transactionManager = context.GetService<ITransactionManager>();

        await transactionManager.FailedAsync(cancellationToken);
    }
}
