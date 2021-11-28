using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

public abstract class DefinitionLibrary<T> : AzureDevOpsDefinition
{
    internal abstract IEnumerable<Conditioned<T>> Items { get; }

    public DefinitionLibrary() : base()
    {
    }
}

public record LibraryReference<T> : Conditioned<T>
{
    internal IEnumerable<Conditioned<T>> Items { get; }

    public LibraryReference(DefinitionLibrary<T> library) : base()
    {
        Items = library.Items;
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
