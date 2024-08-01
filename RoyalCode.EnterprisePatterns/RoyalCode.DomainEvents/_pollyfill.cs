#if NETSTANDARD2_1
namespace System.Runtime.CompilerServices;

sealed class CollectionBuilderAttribute : Attribute
{
    public CollectionBuilderAttribute(Type builderType, string methodName)
    {
        BuilderType = builderType;
        MethodName = methodName;
    }

    public Type BuilderType { get; }
    public string MethodName { get; }
}
#endif
