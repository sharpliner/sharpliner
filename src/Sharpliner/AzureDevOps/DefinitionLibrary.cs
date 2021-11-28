using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Abstract parent for all libraries (re-usable sets of items).
/// </summary>
/// <typeparam name="T">Type of items in the library</typeparam>
public abstract class DefinitionLibrary<T> : AzureDevOpsDefinition
{
    internal abstract IEnumerable<Conditioned<T>> Items { get; }

    internal DefinitionLibrary() : base()
    {
    }
}

/// <summary>
/// This class is used to reference a library in a pipeline.
/// </summary>
/// <typeparam name="T">Type of items in the library</typeparam>
public record LibraryReference<T> : Conditioned<T>
{
    internal IEnumerable<Conditioned<T>> Items { get; }

    public LibraryReference(DefinitionLibrary<T> library) : base()
    {
        Items = library.Items;
    }

    public LibraryReference(IEnumerable<Conditioned<T>> items) : base()
    {
        Items = items;
    }

    protected override void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        foreach (var item in Items)
        {
            nestedObjectSerializer(item);
        }
    }

    internal override void SetIsList(bool isList)
    {
        base.SetIsList(true);

        foreach (var item in Items)
        {
            item.SetIsList(true);
        }
    }
}
