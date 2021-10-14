using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    public abstract class TemplateDefinition<T> : AzureDevOpsDefinitions
    {
        [DisallowNull]
        public virtual List<TemplateParameter> Parameters { get; } = new();

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

        /// <summary>
        /// Disallow any other types than what we define here as AzDO only supports these.
        /// </summary>
        internal TemplateDefinition()
        {
        }

        /// <summary>
        /// Defines a string template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        /// <param name="allowedValues">Allowed list of values (for some data types)</param>
        protected TemplateParameter StringParameter(string name, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
            => new StringTemplateParameter(name, defaultValue, allowedValues);

        /// <summary>
        /// Defines a number template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        /// <param name="allowedValues">Allowed list of values (for some data types)</param>
        protected TemplateParameter NumberParameter(string name, int defaultValue = 0, IEnumerable<int>? allowedValues = null)
            => new NumberTemplateParameter(name, defaultValue, allowedValues);

        /// <summary>
        /// Defines a boolean template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter BooleanParameter(string name, bool defaultValue = false)
            => new BooleanTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a object template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter ObjectParameter(string name, Dictionary<string, object>? defaultValue = null)
            => new ObjectTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a step template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter StepParameter(string name, Step? defaultValue = null)
            => new StepTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a stepList template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter StepListParameter(string name, ConditionedList<Step>? defaultValue = null)
            => new StepListTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a job template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter JobParameter(string name, Job? defaultValue = null)
            => new JobTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a jobList template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter JobListParameter(string name, ConditionedList<Job>? defaultValue = null)
            => new JobListTemplateParameter(name, defaultValue);

        /* TODO: When we have Deployment https://github.com/sharpliner/sharpliner/issues/72
        /// <summary>
        /// Defines a deployment template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter DeploymentParameter(string name, ? defaultValue = null)
            => new DeploymentTemplateParameter(name, defaultValue, null);

        /// <summary>
        /// Defines a deploymentList template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter DeploymentListParameter(string name, ConditionedList<>? defaultValue = null)
            => new DeploymentListTemplateParameter(name, defaultValue, null);
        */

        /// <summary>
        /// Defines a stage template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter StageParameter(string name, Stage? defaultValue = null)
            => new StageTemplateParameter(name, defaultValue);

        /// <summary>
        /// Defines a stageList template parameter
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        protected TemplateParameter StageListParameter(string name, ConditionedList<Stage>? defaultValue = null)
            => new StageListTemplateParameter(name, defaultValue);

        /// <summary>
        /// Allows the ${{ parameters.name }} notation for parameter reference.
        /// </summary>
        protected readonly TemplateParameterReference parameters = new();

        /// <summary>
        /// Allows the ${{ parameters.name }} notation for a stage defined in parameters.
        /// </summary>
        protected Stage StageParameterReference(string parameterName) => new StageParameterReference(parameterName);

        /// <summary>
        /// Allows the ${{ parameters.name }} notation for a job defined in parameters.
        /// </summary>
        protected Job JobParameterReference(string parameterName) => new JobParameterReference(parameterName);

        /// <summary>
        /// Allows the ${{ parameters.name }} notation for a step defined in parameters.
        /// </summary>
        protected Step StepParameterReference(string parameterName) => new StepParameterReference(parameterName);
    }

    /// <summary>
    /// Inherit from this class to define a stage template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class StageTemplateDefinition : TemplateDefinition<Stage>
    {
        internal sealed override string YamlProperty => "stages";
    }

    /// <summary>
    /// Inherit from this class to define a job template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class JobTemplateDefinition : TemplateDefinition<Job>
    {
        internal sealed override string YamlProperty => "jobs";
    }

    /// <summary>
    /// Inherit from this class to define a step template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class StepTemplateDefinition : TemplateDefinition<Step>
    {
        internal sealed override string YamlProperty => "steps";
    }

    /// <summary>
    /// Inherit from this class to define a variable template.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public abstract class VariableTemplateDefinition : TemplateDefinition<VariableBase>
    {
        internal sealed override string YamlProperty => "variables";
    }

    public sealed class TemplateParameterReference
    {
        public string this[string parameterName] => "${{ parameters." + parameterName + " }}";
    }
}
