//HintName: UserProfile.g.cs
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.DESGenerator.Tests.Models.Users
{
    public partial class UserProfile
    {
        [MemberNotNull(nameof(UserName), nameof(Name), nameof(EMail))]
        protected void Apply(global::RoyalCode.DESGenerator.Tests.Models.Users.Events.UserCreated evt)
        {
            AddEvent(evt);
            WhenUserCreated(evt);
        }

        protected void Apply(global::RoyalCode.DESGenerator.Tests.Models.Users.Events.UserNameChanged evt)
        {
            AddEvent(evt);
            WhenUserNameChanged(evt);
        }

    }
}
