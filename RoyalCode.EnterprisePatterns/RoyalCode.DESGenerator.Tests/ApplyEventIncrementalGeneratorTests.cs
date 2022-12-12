
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using RoyalCode.DomainEvents.SourceGenerator;
using System.Runtime.CompilerServices;

namespace RoyalCode.DESGenerator.Tests;

[UsesVerify]
public class ApplyEventIncrementalGeneratorTests
{
    [Fact]
    public async Task SimpleMethodWithWhenAttribute()
    {
        await TestHelper.Verify(CodeForTest.Code);
    }
}

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree });


        // Create an instance of our ApplyEventIncrementalGenerator incremental source generator
        var generator = new ApplyEventIncrementalGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver);
    }
}

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}

public static class CodeForTest
{
    public static string Code = @"

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

public partial class UserProfile : AggregateRoot<int>
{
    public string UserName { get; private set; }

    public string Name { get; private set; }

    public EMail EMail { get; private set; }
   
    [When]
    [MemberNotNull(nameof(UserName), nameof(Name), nameof(EMail))]
    protected internal void WhenUserCreated(Events.UserCreated evt)
    {
        UserName = evt.UserName;
        Name = evt.Name;
        EMail = evt.EMail;
    }

    [WhenAttribute]
    protected internal void WhenUserNameChanged(Events.UserNameChanged evt)
    {
        UserName = evt.UserName;
    }

    [MemberNotNull(nameof(UserName), nameof(Name), nameof(EMail))]
    protected internal void NotToGenerate1(Events.UserCreated evt) { }

    protected internal void NotToGenerate2(Events.UserCreated evt) { }
}

//public partial class UserProfile
//{
//    [When]
//    protected internal void WhenUserNameChanged(Events.UserNameChanged evt)
//    {
//        UserName = evt.UserName;
//    }
//}

public class EMail
{
    public string Value { get; }

    public EMail(string value)
    {
        Value = value;
    }
}

";
    
}