#if !NET11_0_OR_GREATER
namespace System.Runtime.CompilerServices;

[AttributeUsage(Class | Struct, AllowMultiple = false, Inherited = false)]
public sealed class UnionAttribute : Attribute;

public interface IUnion
{
    object? Value { get; }
}
#endif