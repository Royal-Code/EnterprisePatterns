using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Searches.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace RoyalCode.Persistence.Tests.Specifiers;

public class SpecifierFunctionGeneratorTests
{
    [Fact]
    public void Generate_Must_GenerateTheFilterFunction_For_SimpleModel()
    {
        // arrange
        var generator = new DefaultSpecifierFunctionGenerator();

        // act
        var function = generator.Generate<SimpleModel, SimpleFilter>();

        // assert
        Assert.NotNull(function);
    }

    [Fact]
    public void Generate_Must_GenerateTheFilterFunction_For_NullableProperties()
    {
        // arrange
        var generator = new DefaultSpecifierFunctionGenerator();

        // act
        var function = generator.Generate<SimpleModel, NullablePropertiesFilter>();

        // assert
        Assert.NotNull(function);
    }

    [Fact]
    public void Generate_Must_GenerateTheFilter_When_ConfiguredByUnitOfWork()
    {
        // arrange
        ServiceCollection services = new();
        services.AddUnitOfWork<LocalDbContext>()
            .ConfigureDbContext(builder => builder.UseInMemoryDatabase("test"))
            .ConfigureSearches(s => s.Add<SimpleModel>());

        ServiceProvider provider = services.BuildServiceProvider();

        // act
        var search = provider.GetService<ISearch<SimpleModel>>();

        // assert
        Assert.NotNull(search);

        // act
        search!.FilterBy(new SimpleFilter());
        var resultList = search.ToList();

        // assert
        Assert.NotNull(resultList);
    }

    [Fact]
    public void GetMemberAccess_OnProperty_Must_ReturnTheMemberAccess_WithSameType()
    {
        // arrange
        var property = typeof(SimpleModel).GetProperty(nameof(SimpleModel.Guid))!;
        var parameter = Expression.Parameter(typeof(SimpleModel), "x");

        // act
        var memberAccess = DefaultSpecifierFunctionGenerator.GetMemberAccess(property, parameter);

        // assert
        Assert.NotNull(memberAccess);
        Assert.Equal(typeof(Guid), memberAccess.Type);
    }

    [Fact]
    public void GetMemberAccess_OnProperty_Must_ReturnTheMemberAccess_WithUnderlyingType()
    {
        // arrange
        var property = typeof(SimpleModel).GetProperty(nameof(SimpleModel.Date))!;
        var parameter = Expression.Parameter(typeof(SimpleModel), "x");

        // act
        var memberAccess = DefaultSpecifierFunctionGenerator.GetMemberAccess(property, parameter);

        // assert
        Assert.NotNull(memberAccess);
        Assert.Equal(typeof(DateTime), memberAccess.Type);
    }

    [Fact]
    public void GetMemberAccess_PropertySelection_Must_ReturnTheMemberAccess_WithSameType()
    {
        // arrange
        var property = typeof(SimpleModel).SelectProperty(nameof(SimpleModel.Guid))!;
        var parameter = Expression.Parameter(typeof(SimpleModel), "x");

        // act
        var memberAccess = DefaultSpecifierFunctionGenerator.GetMemberAccess(property, parameter);

        // assert
        Assert.NotNull(memberAccess);
        Assert.Equal(typeof(Guid), memberAccess.Type);
    }

    [Fact]
    public void GetMemberAccess_PropertySelection_Must_ReturnTheMemberAccess_WithUnderlyingType()
    {
        // arrange
        var property = typeof(SimpleModel).SelectProperty(nameof(SimpleModel.Date))!;
        var parameter = Expression.Parameter(typeof(SimpleModel), "x");

        // act
        var memberAccess = DefaultSpecifierFunctionGenerator.GetMemberAccess(property, parameter);

        // assert
        Assert.NotNull(memberAccess);
        Assert.Equal(typeof(DateTime), memberAccess.Type);
    }

    [Theory]
    [InlineData(CriterionOperator.Equal)]
    [InlineData(CriterionOperator.In)]
    [InlineData(CriterionOperator.LessThan)]
    [InlineData(CriterionOperator.LessThanOrEqual)]
    [InlineData(CriterionOperator.GreaterThan)]
    [InlineData(CriterionOperator.GreaterThanOrEqual)]
    [InlineData(CriterionOperator.Like)]
    public void DiscoveryCriterionOperator_Must_ReturnSameOperator_WhenOperatorIsNotAuto(CriterionOperator inOperator)
    {
        // arrange
        var criterion = new CriterionAttribute
        {
            Operator = inOperator
        };
        var property = typeof(SimpleModel).GetProperty(nameof(SimpleModel.Name))!;

        // act
        var @operator = DefaultSpecifierFunctionGenerator.DiscoveryCriterionOperator(criterion, property);

        // assert
        Assert.Equal(inOperator, @operator);
    }

    [Theory]
    [InlineData(nameof(MultiFilterTypes.Name), CriterionOperator.Like)]
    [InlineData(nameof(MultiFilterTypes.Age), CriterionOperator.Equal)]
    [InlineData(nameof(MultiFilterTypes.Date), CriterionOperator.Equal)]
    [InlineData(nameof(MultiFilterTypes.Tags), CriterionOperator.In)]
    [InlineData(nameof(MultiFilterTypes.Value), CriterionOperator.Equal)]
    public void DiscoveryCriterionOperator_Must_ReturnDefaultOperator_WhenOperatorIsAuto(
        string propertyName, CriterionOperator expectedOperator)
    {
        // arrange
        var criterion = new CriterionAttribute();
        var property = typeof(MultiFilterTypes).GetProperty(propertyName)!;

        // act
        var @operator = DefaultSpecifierFunctionGenerator.DiscoveryCriterionOperator(criterion, property);

        // assert
        Assert.Equal(expectedOperator, @operator);
    }

    [Theory]
    [InlineData(nameof(MultiFilterTypes.Name), true, ExpressionType.Call, "IsNullOrWhiteSpace")]
    [InlineData(nameof(MultiFilterTypes.Age), false, ExpressionType.MemberAccess, "HasValue")]
    [InlineData(nameof(MultiFilterTypes.Date), true, ExpressionType.Call, "IsBlank")]
    [InlineData(nameof(MultiFilterTypes.Tags), false, ExpressionType.Call, "Any")]
    [InlineData(nameof(MultiFilterTypes.Value), false, ExpressionType.GreaterThan, null)]
    [InlineData(nameof(MultiFilterTypes.Some), false, ExpressionType.NotEqual, null)]
    [InlineData(nameof(MultiFilterTypes.StructValues), true, ExpressionType.Call, "IsEmpty")]
    public void GetIfIsEmptyConstraintExpression_Must_WrapWithIfExpression(
        string propertyName, bool not, ExpressionType expectedType, string? name)
    {
        // arrange
        var assignExpression = Expression.Assign(Expression.Parameter(typeof(bool)), Expression.Constant(true));
        var filterMemberAccess = Expression.MakeMemberAccess(
            Expression.Parameter(typeof(MultiFilterTypes)),
            typeof(MultiFilterTypes).GetProperty(propertyName)!);

        // act
        var expression = DefaultSpecifierFunctionGenerator.GetIfIsEmptyConstraintExpression(
            filterMemberAccess,
            assignExpression);

        // assert
        Assert.Equal(ExpressionType.Conditional, expression.NodeType);

        var condition = ((ConditionalExpression)expression).Test;
        
        if (not)
        {
            Assert.Equal(ExpressionType.Not, condition.NodeType);
            condition = ((UnaryExpression)condition).Operand;
        }

        Assert.Equal(expectedType, condition.NodeType);

        if (name is not null)
        {
            if (expectedType == ExpressionType.Call)
                Assert.Equal(name, ((MethodCallExpression)condition).Method.Name);
            else if (expectedType == ExpressionType.MemberAccess)
                Assert.Equal(name, ((MemberExpression)condition).Member.Name);
        }
    }

    [Theory]
    [InlineData(typeof(Guid?), typeof(Guid), true)]
    [InlineData(typeof(Guid), typeof(Guid?), true)]
    [InlineData(typeof(int), typeof(long), false)]
    [InlineData(typeof(int?), typeof(long), false)]
    [InlineData(typeof(int), typeof(long?), false)]
    [InlineData(typeof(int?), typeof(long?), false)]
    [InlineData(typeof(IEnumerable<int>), typeof(int), true)]
    [InlineData(typeof(IEnumerable<int>), typeof(int?), true)]
    [InlineData(typeof(IEnumerable<int?>), typeof(int?), true)]
    [InlineData(typeof(IEnumerable<int>), typeof(long), false)]
    [InlineData(typeof(IEnumerable), typeof(int), false)]
    [InlineData(typeof(IEnumerable), typeof(int?), false)]
    [InlineData(typeof(IEnumerable), typeof(object), false)]
    [InlineData(typeof(IEnumerable<IEnumerable<int>>), typeof(int), false)]
    public void CheckTypes_Must_ValidateDiferentsTypeAbleToFilter(
        Type filterPropertyType, Type modelPropertyType, bool expected)
    {
        // act
        var match = DefaultSpecifierFunctionGenerator.CheckTypes(filterPropertyType, modelPropertyType);

        // assert
        Assert.Equal(expected, match);
    }

    [Theory]
    [InlineData(CriterionOperator.Equal, false, "Age", ExpressionType.Equal, false)]
    [InlineData(CriterionOperator.Equal, true, "Age", ExpressionType.NotEqual, false)]
    [InlineData(CriterionOperator.GreaterThanOrEqual, false, "Age", ExpressionType.GreaterThanOrEqual, false)]
    [InlineData(CriterionOperator.GreaterThanOrEqual, true, "Age", ExpressionType.GreaterThanOrEqual, true)]
    [InlineData(CriterionOperator.GreaterThan, false, "Age", ExpressionType.GreaterThan, false)]
    [InlineData(CriterionOperator.GreaterThan, true, "Age", ExpressionType.GreaterThan, true)]
    [InlineData(CriterionOperator.LessThanOrEqual, false, "Age", ExpressionType.LessThanOrEqual, false)]
    [InlineData(CriterionOperator.LessThanOrEqual, true, "Age", ExpressionType.LessThanOrEqual, true)]
    [InlineData(CriterionOperator.LessThan, false, "Age", ExpressionType.LessThan, false)]
    [InlineData(CriterionOperator.LessThan, true, "Age", ExpressionType.LessThan, true)]
    [InlineData(CriterionOperator.Like, false, "Name", ExpressionType.Call, false)]
    [InlineData(CriterionOperator.Like, true, "Name", ExpressionType.Call, true)]
    [InlineData(CriterionOperator.Contains, false, "Name", ExpressionType.Call, false)]
    [InlineData(CriterionOperator.Contains, true, "Name", ExpressionType.Call, true)]
    [InlineData(CriterionOperator.StartsWith, false, "Name", ExpressionType.Call, false)]
    [InlineData(CriterionOperator.StartsWith, true, "Name", ExpressionType.Call, true)]
    [InlineData(CriterionOperator.EndsWith, false, "Name", ExpressionType.Call, false)]
    [InlineData(CriterionOperator.EndsWith, true, "Name", ExpressionType.Call, true)]
    [InlineData(CriterionOperator.In, false, "Tag", ExpressionType.Call, false)]
    [InlineData(CriterionOperator.In, true, "Tag", ExpressionType.Call, true)]
    public void CreateOperatorExpression_Must_GenerateTheRelatedExpression(
        CriterionOperator @operator, bool negation, string propertyName, ExpressionType expected, bool not)
    {
        // arrange
        var filterMemberAccess = Expression.MakeMemberAccess(
            Expression.Parameter(typeof(OperatorsFilter)),
            typeof(OperatorsFilter).GetProperty(propertyName)!);
        var modelMemberAccess = Expression.MakeMemberAccess(
            Expression.Parameter(typeof(OperatorsModel)),
            typeof(OperatorsModel).GetProperty(propertyName)!);

        // act
        var expression = DefaultSpecifierFunctionGenerator.CreateOperatorExpression(
            @operator,
            negation,
            filterMemberAccess,
            modelMemberAccess);

        // assert
        if (not)
        {
            Assert.Equal(ExpressionType.Not, expression.NodeType);
            expression = ((UnaryExpression)expression).Operand;
        }
        
        Assert.Equal(expected, expression.NodeType);
    }
}

public class SimpleModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Guid Guid { get; set; }

    public DateTime? Date { get; set; }
}

public class SimpleFilter
{
    public string Name { get; set; }
}

file class NullablePropertiesFilter
{
    public string? Name { get; set; }

    public Guid? Guid { get; set; }
}

file class MultiFilterTypes
{
    public string Name { get; set; }

    public int? Age { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public decimal Value { get; set; }

    public SomeValueObject? Some { get; set; }

    public SomeStructValues StructValues { get; set; }
}

file record SomeValueObject(string Name, int Age);

file struct SomeStructValues
{
    public string Name { get; set; }

    public int Age { get; set; }
}

file class OperatorsModel
{
    public string Name { get; set; }

    public int Age { get; set; }

    public string Tag { get; set; }
}

file class OperatorsFilter
{
    public string Name { get; set; }

    public int Age { get; set; }

    public IEnumerable<string> Tag { get; set; }
}

file class LocalDbContext : DbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SimpleModel>();
    }
}