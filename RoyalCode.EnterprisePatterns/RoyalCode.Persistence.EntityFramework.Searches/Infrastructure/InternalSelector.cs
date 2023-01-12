﻿
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RoyalCode.Persistence.EntityFramework.Searches.Infrastructure;

internal class InternalSelector<TEntity, TDto> : ISelector<TEntity, TDto>
     where TEntity : class
        where TDto : class
{
    public Expression<Func<TEntity, TDto>> Selector { get; }

    public InternalSelector(Expression<Func<TEntity, TDto>> selector)
    {
        Selector = selector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Expression<Func<TEntity, TDto>> GetSelectExpression() => Selector;
}