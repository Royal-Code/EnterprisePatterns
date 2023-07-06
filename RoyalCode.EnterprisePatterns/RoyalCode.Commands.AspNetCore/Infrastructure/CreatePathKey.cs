namespace RoyalCode.Commands.AspNetCore.Infrastructure;

internal struct CreatePathKey
{
    public Type EntityType { get; set; }

    public Type ModelType { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CreatePathKey key &&
               EqualityComparer<Type>.Default.Equals(EntityType, key.EntityType) &&
               EqualityComparer<Type>.Default.Equals(ModelType, key.ModelType);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EntityType, ModelType);
    }
}

internal static class CreatePathKey<TEntity, TModel>
{
    public static string? Path { get; set; }

    public static Func<TEntity, string, string>? Formatter { get; set; }
}
