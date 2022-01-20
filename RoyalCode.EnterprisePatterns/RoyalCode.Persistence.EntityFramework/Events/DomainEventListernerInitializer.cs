using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics;

namespace RoyalCode.Persistence.EntityFramework.Events;

public class DomainEventListernerInitializer : IUnitOfWorkInitializeInterceptor
{
    public void Initializing(DbContext context)
    {

        var listener = context.GetService<DomainEventListerner>();
        if (listener is null)
            throw new InvalidOperationException(DomainEventResources.DomainEventServiceNotFound);

        context.ChangeTracker.Tracked += listener.EntityTracked;
        context.SavingChanges += listener.Saving;
        context.SavedChanges += listener.Saved;
    }
}

public class DomainEventListerner
{
    public void EntityTracked(object? sender, EntityTrackedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void Saving(object? sender, SavingChangesEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void Saved(object? sender, SavedChangesEventArgs e)
    {
        throw new NotImplementedException();
    }
}