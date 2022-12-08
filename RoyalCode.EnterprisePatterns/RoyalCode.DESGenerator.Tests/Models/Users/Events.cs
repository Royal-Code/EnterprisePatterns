
using RoyalCode.DomainEvents;
using System.Text.Json.Serialization;

namespace RoyalCode.DESGenerator.Tests.Models.Users;

public static class Events
{

    public class UserCreated : DomainEventBase, ICreationEvent
    {
        private readonly UserProfile? userProfile;
        private readonly int userProfileId;

        public int UserProfileId => userProfile?.Id ?? userProfileId;

        public string UserName { get; private set; }

        public string Name { get; private set; }

        public EMail EMail { get; private set; }

        public UserCreated(UserProfile userProfile)
        {
            this.userProfile = userProfile;
            UserName = userProfile.UserName;
            Name = userProfile.Name;
            EMail = userProfile.EMail;
        }

        [JsonConstructor]
        public UserCreated(
            Guid id, DateTimeOffset occurred,
            int userProfileId, string userName, string name, EMail eMail)
            : base(id, occurred)
        {
            this.userProfileId = userProfileId;
            UserName = userName;
            Name = name;
            EMail = eMail;
        }

        public void Saved() { }
    }

    public class UserNameChanged
    {
        public string UserName { get; internal set; }
    }
}
