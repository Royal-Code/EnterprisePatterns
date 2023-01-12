using RoyalCode.Persistence.Searches.Abstractions.Linq;
using System.Linq.Expressions;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

public interface ISelectorGenerator
{
    ISelector<TEntity, TDto> Generate<TEntity, TDto>()
        where TEntity : class
        where TDto : class;
}
