using RoyalCode.SmartCommands;
using RoyalCode.SmartProblems;

namespace RoyalCode.Examples.Blogs.Contracts;

[MapGroup("Hello")]
[MapPost("/world", "Execute a command Hello World")]
public partial class Command
{
    public string? Name { get; set; }

    [Command]
    public Result Execute()
    {
        Console.WriteLine($"Executing command: {Name}");

        // Simulate command execution
        return Result.Ok();
    }
}
