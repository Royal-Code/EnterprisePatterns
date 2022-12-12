//HintName: WhenAttribute.g.cs

using System;
namespace RoyalCode.DomainEvents
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class WhenAttribute : Attribute
    {
        public WhenAttribute() { }
    }
}
