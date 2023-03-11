using RoyalCode.Extensions.PropertySelection;
using RoyalCode.Persistence.Searches.Abstractions.Extensions;
using System.Linq.Expressions;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Selector.Converters;

internal class NullableSelectorPropertyConverter : ISelectorPropertyConverter
{
    public bool CanConvert(PropertyMatch selection)
    {
        // check if the target (entity) property is nullable and the origin (DTO) property is not nullable.
        return selection.TargetSelection!.PropertyType.IsNullableType()
            && selection.OriginProperty.PropertyType.IsNotNullableType();
    }

    public Expression GetExpression(PropertyMatch selection, Expression parameter)
    {
        // generate a expression like:
        // e.Id.HasValue ? e.Id.Value : default(int)
        // where:
        // e is the parameter
        // Id is the Selection property,
        // int is the origen property type.

        var hasValueExpression = selection.TargetSelection!.SelectChild(nameof(Nullable<int>.HasValue))!.GetAccessExpression(parameter);
        var valueExpression = selection.TargetSelection!.SelectChild(nameof(Nullable<int>.Value))!.GetAccessExpression(parameter);
        var defaultExpression = Expression.Default(selection.OriginProperty.PropertyType);
        var ifExpression = Expression.Condition(hasValueExpression, valueExpression, defaultExpression);

        return ifExpression;
    }
}