using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    public abstract class TemplateDefinition<T> : AzureDevOpsDefinitions
    {
        [DisallowNull]
        public abstract List<TemplateParameterDefinition> Parameters { get; }

        [DisallowNull]
        public abstract ConditionedList<T> Definition { get; }

        internal abstract string YamlProperty { get; }

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

        /// <summary>
        /// Disallow any other types than what we define here as AzDO only supports these.
        /// </summary>
        internal TemplateDefinition()
        {
        }
    }

    /// <summary>
    /// Inherit from this class to define a stage template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class StageTemplateDefinition : TemplateDefinition<Stage>
    {
        internal sealed override string YamlProperty => "stages";
    }

    /// <summary>
    /// Inherit from this class to define a job template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class JobTemplateDefinition : TemplateDefinition<Job>
    {
        internal sealed override string YamlProperty => "jobs";
    }

    /// <summary>
    /// Inherit from this class to define a step template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class StepTemplateDefinition : TemplateDefinition<Step>
    {
        internal sealed override string YamlProperty => "steps";
    }

    /// <summary>
    /// Inherit from this class to define a variable template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class VariableTemplateDefinition : TemplateDefinition<VariableBase>
    {
        internal sealed override string YamlProperty => "variables";
    }
}
