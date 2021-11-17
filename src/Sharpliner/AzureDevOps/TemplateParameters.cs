using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This class allows you to pass parameters to to templates.
/// To nest objects, insert another TemplateParameters value.
/// You can also specify a condition in the key and nest values conditionally.
/// </summary>
public class TemplateParameters : Dictionary<string, object>, IYamlConvertible
{
    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart());

        foreach (var parameter in this)
        {
            emitter.Emit(new Scalar(parameter.Key));
            nestedObjectSerializer(parameter.Value);
        }

        emitter.Emit(new MappingEnd());
    }
}
