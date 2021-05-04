using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.Model
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

        protected override void SerializeContent(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new Scalar("template"));
            emitter.Emit(new Scalar(_path));

            if (Parameters != null && Parameters.Any())
            {
                emitter.Emit(new Scalar("parameters"));
                nestedObjectSerializer(Parameters);
            }
        }
    }

    public static class TemplateDefinitionExtensions
    {
        public static ConditionedDefinition<T> Template<T>(this Condition<T> condition, string path, TemplateParameters parameters)
            => ConditionedDefinition.Link(condition, new Template<T>(condition: condition.ToString(), path: path, parameters));

        public static ConditionedDefinition<T> Template<T>(this ConditionedDefinition<T> conditionedDefinition, string path, TemplateParameters parameters)
        {
            var template = new Template<T>(path: path, parameters);
            conditionedDefinition.Definitions.Add(template);
            return conditionedDefinition;
        }
    }

    public class TemplateParameters : Dictionary<string, object> { }
}
