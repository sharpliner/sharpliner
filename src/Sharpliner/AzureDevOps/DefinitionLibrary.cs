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
    internal DefinitionLibrary<T> Library { get; }

    public LibraryReference(DefinitionLibrary<T> library) : base()
    {
        Library = library;
    }

    public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        foreach (var item in Library.Items)
        {
            nestedObjectSerializer(item);
        }
    }
}
