
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

        public UserCreated(UserProfile userProfile, string userName, string name, EMail eMail)
        {
            this.userProfile = userProfile;
            UserName = userName;
            Name = name;
            EMail = eMail;
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

    public class UserNameChanged : DomainEventBase
    {
        public string UserName { get; }

        public UserNameChanged(string userName)
        {
            UserName = userName;
        }

        [JsonConstructor]
        public UserNameChanged(
            Guid id, DateTimeOffset occurred,
            string userName)
            : base(id, occurred)
        {
            UserName = userName;
        }
    }
}
