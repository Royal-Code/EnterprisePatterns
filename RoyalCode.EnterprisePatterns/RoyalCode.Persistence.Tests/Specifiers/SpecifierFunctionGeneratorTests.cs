

using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using Xunit;

namespace RoyalCode.Persistence.Tests.Specifiers;

public class SpecifierFunctionGeneratorTests
{
    [Fact]
    public void Generate_Must_GenerateTheFilterFunction_For_SimpleModel()
    {
        var generator = new DefaultSpecifierFunctionGenerator();

        var function = generator.Generate<SimpleModel, SimpleFilter>();

        Assert.NotNull(function);
    }
}

public class SimpleModel
{
    public string Name { get; set; }
}

public class SimpleFilter
{
    public string Name { get; set; }
}