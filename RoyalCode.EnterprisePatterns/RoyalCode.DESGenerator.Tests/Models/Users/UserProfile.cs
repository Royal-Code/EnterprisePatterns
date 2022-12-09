using RoyalCode.DomainEvents;
using RoyalCode.Aggregates;

namespace RoyalCode.DESGenerator.Tests.Models.Users;

public partial class UserProfile : AggregateRoot<int>
{
    public string UserName { get; private set; }

    public string Name { get; private set; }

    public EMail EMail { get; private set; }

    public UserProfile(string userName, string name, EMail eMail)
    {
        UserName = userName;
        Name = name;
        EMail = eMail;
        
        
    }
    
    [When]
    protected internal void WhenUserCreated(Events.UserCreated evt)
    {
        UserName = evt.UserName;
        Name = evt.Name;
        EMail = evt.EMail;
    }
}

public partial class UserProfile
{
    [When]
    protected internal void WhenUserNameChanged(Events.UserNameChanged evt)
    {
        UserName = evt.UserName;
    }
}