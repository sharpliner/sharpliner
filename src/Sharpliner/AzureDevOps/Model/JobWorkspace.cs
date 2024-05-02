using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

public class JobWorkspace : IYamlConvertible
{
    public static readonly JobWorkspace Outputs = new("outputs");
    public static readonly JobWorkspace Resources = new("resources");
    public static readonly JobWorkspace All = new("all");

    private readonly string _cleanTarget;

    private JobWorkspace(string cleanTarget)
    {
        _cleanTarget = cleanTarget;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart());
        emitter.Emit(new Scalar("clean"));
        emitter.Emit(new Scalar(_cleanTarget));
        emitter.Emit(new MappingEnd());
    }
}
