﻿using Microsoft.EntityFrameworkCore;
using RoyalCode.Searches.Abstractions;

#pragma warning disable S2326 // Unused type parameters should be removed

namespace RoyalCode.Searches.Persistence.EntityFramework;

/// <summary>
/// <para>
///     Represents a search component for a persistence unit defined by a <see cref="DbContext"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">
///     The <see cref="DbContext"/> type that defines the persistence unit.
/// </typeparam>
public interface ISearchable<TDbContext> : ISearchable
    where TDbContext : DbContext
{ }