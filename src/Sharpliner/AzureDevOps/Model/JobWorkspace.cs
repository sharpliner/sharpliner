using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Specifies the workspace to clean before the job runs.
/// </summary>
public class JobWorkspace : IYamlConvertible
{
    /// <summary>
    /// Clean all outputs in the workspace before the job runs.
    /// </summary>
    public static JobWorkspace Outputs { get; } = new("outputs");

    /// <summary>
    /// Clean all resources in the workspace before the job runs.
    /// </summary>
    public static JobWorkspace Resources { get; } = new("resources");

    /// <summary>
    /// Clean all outputs and resources in the workspace before the job runs.
    /// </summary>
    public static JobWorkspace All { get; } = new("all");

    private readonly string _cleanTarget;

    private JobWorkspace(string cleanTarget)
    {
        _cleanTarget = cleanTarget;
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart());
        emitter.Emit(new Scalar("clean"));
        emitter.Emit(new Scalar(_cleanTarget));
        emitter.Emit(new MappingEnd());
    }
}
