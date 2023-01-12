using System.Linq.Expressions;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

public interface ISelectorExpressionGenerator
{
    Expression<Func<TEntity, TDto>> Generate<TEntity, TDto>()
        where TEntity : class
        where TDto : class;
}