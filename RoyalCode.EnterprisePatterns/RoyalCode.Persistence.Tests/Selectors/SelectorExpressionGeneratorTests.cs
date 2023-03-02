
using RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;
using Xunit;

namespace RoyalCode.Persistence.Tests.Selectors;

public class SelectorExpressionGeneratorTests
{
    [Fact]
    public void Generate_Must_GenerateExpression_For_SameProperties()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<SimpleEntity, SimpleDto>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new SimpleEntity { Id = 1, Name = "Name" };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Id, dto!.Id);
        Assert.Equal(entity.Name, dto.Name);
    }

    [Fact]
    public void Generate_Must_GenerateExpression_For_NavegationProperties()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<ComplexEntity, DtoForComplex>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new ComplexEntity()
        {
            Name = "Name",
            Complex = new ComplexType()
            {
                Value = 1
            }
        };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.Complex.Value, dto.ComplexValue);
    }
}

file class SimpleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

file class SimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

file class ComplexEntity
{
    public string Name { get; set; } = null!;

    public ComplexType Complex { get; set; } = null!;
}

file class ComplexType
{
    public int Value { get; set; }
}

file class DtoForComplex
{
    public string Name { get; set; } = null!;

    public int ComplexValue { get; set; }
}