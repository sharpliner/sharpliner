using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public record Template<T> : ConditionedDefinition<T>
    {
        private readonly string _path;
        public TemplateParameters Parameters { get; init; } = new();

        public Template(string path, TemplateParameters? parameters = null) : this(null, path, parameters)
        {
        }

        internal Template(string? condition, string path, TemplateParameters? parameters = null) : base(condition)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            _path = path;

            if (parameters != null)
            {
                Parameters = parameters;
            }
        }

        protected override void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("template"));
            emitter.Emit(new Scalar(_path));

            if (Parameters != null && Parameters.Any())
            {
                emitter.Emit(new Scalar("parameters"));
                emitter.Emit(new MappingStart());

                foreach (var parameter in Parameters)
                {
                    emitter.Emit(new Scalar(parameter.Key));
                    nestedObjectSerializer(parameter.Value);
                }

                emitter.Emit(new MappingEnd());
            }

            emitter.Emit(new MappingEnd());
        }
    }

    public class TemplateParameters : Dictionary<string, object> { }
}
