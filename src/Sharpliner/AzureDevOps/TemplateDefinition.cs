using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This class is just a simple collection of handy macros that make template definition easier.
/// </summary>
public abstract class TemplateDefinition : AzureDevOpsDefinition
{
    /// <summary>
    /// Defines a string template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    protected static TemplateParameter StringParameter(string name, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        => new StringTemplateParameter(name, null, defaultValue, allowedValues);

    /// <summary>
    /// Defines a number template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    protected static TemplateParameter NumberParameter(string name, int defaultValue = 0, IEnumerable<int>? allowedValues = null)
        => new NumberTemplateParameter(name, null, defaultValue, allowedValues);

    /// <summary>
    /// Defines a boolean template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter BooleanParameter(string name, bool defaultValue = false)
        => new BooleanTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a object template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter ObjectParameter(string name, ConditionedDictionary? defaultValue = null)
        => new ObjectTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a step template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter StepParameter(string name, Step? defaultValue = null)
        => new StepTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stepList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter StepListParameter(string name, ConditionedList<Step>? defaultValue = null)
        => new StepListTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter JobParameter(string name, JobBase? defaultValue = null)
        => new JobTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a jobList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter JobListParameter(string name, ConditionedList<JobBase>? defaultValue = null)
        => new JobListTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a deployment job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter DeploymentParameter(string name, DeploymentJob? defaultValue = null)
        => new DeploymentTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a deploymentList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter DeploymentListParameter(string name, ConditionedList<DeploymentJob>? defaultValue = null)
        => new DeploymentListTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stage template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter StageParameter(string name, Stage? defaultValue = null)
        => new StageTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stageList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static TemplateParameter StageListParameter(string name, ConditionedList<Stage>? defaultValue = null)
        => new StageListTemplateParameter(name, null, defaultValue);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a stage defined in parameters.
    /// </summary>
    protected static Stage StageParameterReference(string parameterName) => new StageParameterReference(parameterName);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a job defined in parameters.
    /// </summary>
    protected static JobBase JobParameterReference(string parameterName) => new JobParameterReference(parameterName);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a step defined in parameters.
    /// </summary>
    protected static Step StepParameterReference(string parameterName) => new StepParameterReference(parameterName);

    public sealed class TemplateParameterReference
    {
        public string this[string parameterName] => "${{ parameters." + parameterName + " }}";
    }
}

/// <summary>
/// This is the ancestor of all definitions that produce a Azure pipelines template.
/// </summary>
/// <typeparam name="T">Type of the part of the pipeline that this template is for (one of stages, steps, jobs or variables)</typeparam>
public abstract class TemplateDefinition<T> : TemplateDefinition, ISharplinerDefinition
{
    /// <summary>
    /// Path to the YAML file where this template will be exported to.
    /// When you build the project, the template will be saved into a file on this path.
    /// Example: "pipelines/ci.yaml"
    /// </summary>
    public abstract string TargetFile { get; }

    public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

    [DisallowNull]
    public virtual List<TemplateParameter> Parameters { get; } = new();

    [DisallowNull]
    public abstract ConditionedList<T> Definition { get; }

    internal abstract string YamlProperty { get; }

    public string Serialize()
    {
        var template = new ConditionedDictionary();

        if (Parameters != null && Parameters.Any())
        {
            template.Add("parameters", Parameters);
        }

        template.Add(YamlProperty, Definition);

        return SharplinerSerializer.Serialize(template);
    }

    public abstract IReadOnlyCollection<IDefinitionValidation> Validations { get; }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// 
    /// Leave empty array to omit file header.
    /// </summary>
    public virtual string[]? Header => SharplinerPublisher.GetDefaultHeader(GetType());

    /// <summary>
    /// Disallow any other types than what we define here as AzDO only supports these.
    /// </summary>
    internal TemplateDefinition()
    {
    }
}
