using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

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
    protected TemplateParameter JobParameter(string name, JobBase? defaultValue = null)
        => new JobTemplateParameter(name, defaultValue);

    /// <summary>
    /// Defines a jobList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected TemplateParameter JobListParameter(string name, ConditionedList<JobBase>? defaultValue = null)
        => new JobListTemplateParameter(name, defaultValue);

    /// <summary>
    /// Defines a deployment job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected TemplateParameter DeploymentParameter(string name, DeploymentJob? defaultValue = null)
        => new DeploymentTemplateParameter(name, defaultValue);

    /// <summary>
    /// Defines a deploymentList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected TemplateParameter DeploymentListParameter(string name, ConditionedList<DeploymentJob>? defaultValue = null)
        => new DeploymentListTemplateParameter(name, defaultValue);

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
    protected JobBase JobParameterReference(string parameterName) => new JobParameterReference(parameterName);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a step defined in parameters.
    /// </summary>
    protected Step StepParameterReference(string parameterName) => new StepParameterReference(parameterName);
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
        var template = new Dictionary<string, object>();

        if (Parameters != null && Parameters.Any())
        {
            template.Add("parameters", Parameters);
        }

        template.Add(YamlProperty, Definition);

        return SharplinerSerializer.Serialize(template);
    }

    public void Validate()
    {
    }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// 
    /// Leave empty array to omit file header.
    /// </summary>
    public virtual string[]? Header => ISharplinerDefinition.GetDefaultHeader(GetType());

    /// <summary>
    /// Disallow any other types than what we define here as AzDO only supports these.
    /// </summary>
    internal TemplateDefinition()
    {
    }
}

public abstract class StepTemplateDefinitionCollection : TemplateDefinition, ISharplinerDefinitionCollection
{
    public abstract IEnumerable<TemplateDefinitionData<Step>> Templates { get; }

    public IEnumerable<ISharplinerDefinition> Definitions => Templates.Select(t => new StepTemplateDefinitionWrapper(
        t.TargetFile, t.PathType, t.Definition, t.Parameters ?? new List<TemplateParameter>(), t.Header));

    // Only us inheriting from this
    internal StepTemplateDefinitionCollection()
    {
    }

    private class StepTemplateDefinitionWrapper : StepTemplateDefinition
    {
        private readonly string[]? _header;

        public StepTemplateDefinitionWrapper(
            string targetFile,
            TargetPathType pathType,
            ConditionedList<Step> definition,
            List<TemplateParameter> parameters,
            string[]? header = null)
        {
            TargetFile = targetFile;
            Definition = definition;
            TargetPathType = pathType;
            Parameters = parameters;
            _header = header;
        }

        public override string TargetFile { get; }

        public override TargetPathType TargetPathType { get; }

        public override ConditionedList<Step> Definition { get; }

        public override List<TemplateParameter> Parameters { get; }

        public override string[]? Header => _header ?? base.Header;
    }
}

/// <summary>
/// Use this class to dynamically populate a template definition.
/// </summary>
/// <typeparam name="T">Type of the template</typeparam>
/// <param name="TargetFile"></param>
/// <param name="Definition"></param>
/// <param name="Parameters"></param>
/// <param name="PathType"></param>
/// <param name="Header"></param>
public record TemplateDefinitionData<T>(
    string TargetFile,
    ConditionedList<T> Definition,
    List<TemplateParameter>? Parameters = null,
    TargetPathType PathType = TargetPathType.RelativeToGitRoot,
    string[]? Header = null);

public sealed class TemplateParameterReference
{
    public string this[string parameterName] => "${{ parameters." + parameterName + " }}";
}
