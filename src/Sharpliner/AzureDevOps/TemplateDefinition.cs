using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    public abstract class TemplateDefinition<T> : AzureDevOpsDefinitions
    {
        [DisallowNull]
        public abstract TemplateParameters Parameters { get; }

        [DisallowNull]
        public abstract ConditionedDefinitionList<T> Definition { get; }

        protected abstract string YamlProperty { get; }

        public sealed override string Serialize()
        {
            var template = new Dictionary<string, object>();

            if (Parameters != null && Parameters.Any())
            {
                template.Add("parameters", Parameters);
            }

            template.Add(YamlProperty, Definition);

            return PrettifyYaml(SharplinerSerializer.Serialize(template));
        }

        public sealed override void Validate() { }

        protected static TemplateParameters NoParameters { get; } = new();
    }

    public abstract class StageTemplateDefinition : TemplateDefinition<Stage>
    {
        protected sealed override string YamlProperty => "stages";
    }

    public abstract class JobTemplateDefinition : TemplateDefinition<Job>
    {
        protected sealed override string YamlProperty => "jobs";
    }

    public abstract class StepTemplateDefinition : TemplateDefinition<Step>
    {
        protected sealed override string YamlProperty => "steps";
    }
}
