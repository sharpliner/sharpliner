using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// No-op job is used when other jobs might be omitted based on a condition and no job is left in the stage.
/// </summary>
public record NoopJob : Job, IYamlConvertible
{
    public NoopJob() : base("No_op")
    {
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart());
        emitter.Emit(new Scalar("job"));
        emitter.Emit(new Scalar("No_op"));
        emitter.Emit(new MappingEnd());
    }
}
