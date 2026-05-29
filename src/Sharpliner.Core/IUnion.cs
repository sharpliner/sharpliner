#if !NET11_0_OR_GREATER
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class UnionAttribute : Attribute;

internal interface IUnion
{
    object? Value { get; }
}
#endif
