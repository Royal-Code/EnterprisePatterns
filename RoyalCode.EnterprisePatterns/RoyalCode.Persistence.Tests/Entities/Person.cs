using RoyalCode.Entities;

namespace RoyalCode.Persistence.Tests.Entities;

public class Person : Entity<int>
{
    public string Name { get; set; }
}