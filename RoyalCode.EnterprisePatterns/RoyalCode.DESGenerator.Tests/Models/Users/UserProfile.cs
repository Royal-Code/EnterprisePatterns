using RoyalCode.DomainEvents;
using RoyalCode.Aggregates;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.DESGenerator.Tests.Models.Users;

public partial class UserProfile : AggregateRoot<int>
{
    public string UserName { get; private set; }

    public string Name { get; private set; }

    public EMail EMail { get; private set; }

    public UserProfile(string userName, string name, EMail eMail)
    {
        Apply(new Events.UserCreated(this, userName, name, eMail));
    }

    public void ChangeUserName(string name)
    {
        Apply(new Events.UserNameChanged(name));
    }

    [When]
    [MemberNotNull(nameof(UserName), nameof(Name), nameof(EMail))]
    protected internal void WhenUserCreated(Events.UserCreated evt)
    {
        UserName = evt.UserName;
        Name = evt.Name;
        EMail = evt.EMail;
    }

    [When]
    protected internal void WhenUserNameChanged(Events.UserNameChanged evt)
    {
        UserName = evt.UserName;
    }
}

//public partial class UserProfile
//{
//    [When]
//    protected internal void WhenUserNameChanged(Events.UserNameChanged evt)
//    {
//        UserName = evt.UserName;
//    }
//}