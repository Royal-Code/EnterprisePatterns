namespace RoyalCode.DomainEvents.SourceGenerator;

internal static class AttributeGenerator
{
    internal const string AttributeText = @"
using System;
namespace RoyalCode.DomainEvents
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class WhenAttribute : Attribute
    {
        public WhenAttribute() { }
    }
}
";
}

