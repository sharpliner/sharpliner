using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

public interface IDependsOn
{
    string Name { get; }
    ConditionedList<string> DependsOn { get; }
}

/// <summary>
/// AzDO allows an empty dependsOn which then forces the stage/job to kick off in parallel.
/// If dependsOn is omitted, stages/jobs run in the order they are defined.
/// </summary>
internal class EmptyDependsOn : ConditionedList<string>, IYamlConvertible
{
    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    // We want to write "dependsOn: " (empty value)
    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => emitter.Emit(new Scalar(string.Empty));

    public EmptyDependsOn()
    {
        Add(string.Empty);
    }
}
