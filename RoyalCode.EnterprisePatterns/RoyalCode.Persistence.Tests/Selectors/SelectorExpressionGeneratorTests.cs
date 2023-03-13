
using RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void Generate_Must_GenerateExpression_For_NullableProperties()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<EntityWithNullable, DtoWithoutNullable>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new EntityWithNullable() { Id = 1 };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Id, dto!.Id);

        var q = new List<EntityWithNullable>().AsQueryable();
        q.Select(e => new DtoWithoutNullable()
        {
            Id = e.Id.HasValue ? e.Id.Value : default(int),
            
        });
    }

    [Fact]
    public void Generate_Must_GenerateExpression_For_Different_Enums()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<EntityWithEnum, DtoWithEnum>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new EntityWithEnum() { Id = 1, Enum = DomainValues.Value1 };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Id, dto!.Id);
        Assert.Equal((int)entity.Enum, (int)dto.Enum);
    }

    [Fact]
    public void Generate_Must_GenerateExpression_For_SubSelect()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<EntityForSubSelect, DtoForSubSelect>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new EntityForSubSelect()
        {
            Id = 1,
            SubValue = new EntitySubType()
            {
                Id = 2,
                Name = "Name"
            }
        };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Id, dto!.Id);
        Assert.Equal(entity.SubValue.Id, dto.SubValue.Id);
        Assert.Equal(entity.SubValue.Name, dto.SubValue.Name);
    }

    [Fact]
    public void Generate_Must_GenerateExpression_For_MultiLevelSubSelect()
    {
        // Arrange
        var generator = new DefaultSelectorExpressionGenerator();

        // Act
        var expression = generator.Generate<EntityForMultiLevelSubSelect, DtoForMultiLevelSubSelect>();

        // Assert
        Assert.NotNull(expression);

        // Arrange
        var entity = new EntityForMultiLevelSubSelect()
        {
            Id = 1,
            Value = new EntityForSubSelect()
            {
                Id = 2,
                SubValue = new EntitySubType()
                {
                    Id = 3,
                    Name = "Name"
                }
            }
        };

        // Act
        var dto = expression!.Compile()(entity);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(entity.Id, dto!.Id);
        Assert.Equal(entity.Value.Id, dto.Value.Id);
        Assert.Equal(entity.Value.SubValue.Id, dto.Value.SubValue.Id);
        Assert.Equal(entity.Value.SubValue.Name, dto.Value.SubValue.Name);
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

file class EntityWithNullable
{
    public int? Id { get; set; }
}

file class DtoWithoutNullable
{
    public int Id { get; set; }
}

file class EntityWithEnum
{
    public int Id { get; set; }
    public DomainValues Enum { get; set; }
}

file enum DomainValues
{
    Value1,
    Value2
}

file class DtoWithEnum
{
    public int Id { get; set; }
    public DtosValues Enum { get; set; }
}

file enum DtosValues
{
    DtoValue1,
    DtoValue2
}

file class EntityForSubSelect
{
    public int Id { get; set; }

    public EntitySubType SubValue { get; set; }
}

file class EntitySubType
{
    public int Id { get; set; }

    public string Name { get; set; }

    // a nullable entity property and a non-nullable dto property
    public int? Money { get; set; }

    public string OtherProperty { get; set; }
}

file class DtoForSubSelect
{
    public int Id { get; set; }

    public DtoSubType SubValue { get; set; }
}

file class DtoSubType
{
    public int Id { get; set; }

    // a dto property with ? and a non-nullable entity property
    public string? Name { get; set; }

    public int Money { get; set; }
}

file class EntityForMultiLevelSubSelect
{
    public int Id { get; set; }

    // a nullable entity property and a non-nullable dto property
    public EntityForSubSelect? Value { get; set; }
}

file class DtoForMultiLevelSubSelect
{
    public int Id { get; set; }

    public DtoForSubSelect Value { get; set; }
}