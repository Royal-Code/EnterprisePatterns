using RoyalCode.Extensions.PropertySelection;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Selector.Converters;

internal class SubSelectSelectorPropertyResolver : ISelectorPropertyResolver
{
    public bool CanConvert(PropertyMatch selection, ISelectResolver resolver, out ISelectorPropertyConverter? converter)
    {
        // check if the target (entity) property and the origin (DTO) property
        // are classes but not IEnumerable.
        var areClasses = selection.TargetSelection!.PropertyType.IsClass 
            && selection.OriginProperty.PropertyType.IsClass
            && selection.TargetSelection.PropertyType.IsAssignableTo(typeof(IEnumerable)) is false
            && selection.OriginProperty.PropertyType.IsAssignableTo(typeof(IEnumerable)) is false;

        if (areClasses)
        {
            var resolutions = resolver.GetResolutions(
                selection.TargetSelection.PropertyType,
                selection.OriginProperty.PropertyType,
                out var ctor);

            if (resolutions is not null && ctor is not null)
            {
                converter = new Converter(resolutions, ctor);
                return true;
            }
        }

        converter = null;
        return false;
    }

    private class Converter : ISelectorPropertyConverter
    {
        private readonly IEnumerable<SelectResolution> resolutions;
        private readonly ConstructorInfo ctor;

        public Converter(IEnumerable<SelectResolution> resolutions, ConstructorInfo ctor)
        {
            this.resolutions = resolutions;
            this.ctor = ctor;
        }

        public Expression GetExpression(PropertyMatch selection, Expression parameter)
        {
            // generate de bindings for create the new DTO
            var bindings = resolutions
                .Select(r => Expression.Bind(r.Match.OriginProperty, r.Converter.GetExpression(r.Match, parameter)));

            // create the new DTO with the bindings.
            var newDto = Expression.MemberInit(Expression.New(ctor), bindings);

            return newDto;
        }
    }
}