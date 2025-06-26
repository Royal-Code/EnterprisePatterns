using RoyalCode.Entities;

namespace RoyalCode.Persistence.Tests.Entities;

public class Person : Entity<int>
{
    public string Name { get; set; } = null!;
}

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}