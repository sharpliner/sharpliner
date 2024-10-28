﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;
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
    protected static Parameter StringParameter(string name, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        => new StringParameter(name, null, defaultValue, allowedValues);

    /// <summary>
    /// Defines a number template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    protected static Parameter NumberParameter(string name, int? defaultValue = null, IEnumerable<int?>? allowedValues = null)
        => new NumberParameter(name, null, defaultValue, allowedValues);

    /// <summary>
    /// Defines a boolean template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter BooleanParameter(string name, bool? defaultValue = null)
        => new BooleanParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a object template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter ObjectParameter(string name, ConditionedDictionary? defaultValue = null)
        => new ObjectParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a step template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter StepParameter(string name, Step? defaultValue = null)
        => new StepParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stepList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter StepListParameter(string name, ConditionedList<Step>? defaultValue = null)
        => new StepListParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter JobParameter(string name, JobBase? defaultValue = null)
        => new JobParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a jobList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter JobListParameter(string name, ConditionedList<JobBase>? defaultValue = null)
        => new JobListParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a deployment job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter DeploymentParameter(string name, DeploymentJob? defaultValue = null)
        => new DeploymentParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a deploymentList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter DeploymentListParameter(string name, ConditionedList<DeploymentJob>? defaultValue = null)
        => new DeploymentListParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stage template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter StageParameter(string name, Stage? defaultValue = null)
        => new StageParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stageList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter StageListParameter(string name, ConditionedList<Stage>? defaultValue = null)
        => new StageListParameter(name, null, defaultValue);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a stage defined in parameters.
    /// </summary>
    protected static Stage StageParameterReference(string parameterName) => new StageReference(parameterName);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a job defined in parameters.
    /// </summary>
    protected static JobBase JobParameterReference(string parameterName) => new JobReference(parameterName);

    /// <summary>
    /// Allows the ${{ parameters.name }} notation for a step defined in parameters.
    /// </summary>
    protected static Step StepParameterReference(string parameterName) => new StepReference(parameterName);

    public sealed class TemplateParameterReference
    {
        public ParameterReference this[string parameterName] => new(parameterName);
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
    /// Example: <c>"pipelines/ci.yaml"</c>
    /// </summary>
    public abstract string TargetFile { get; }

    /// <summary>
    /// Specifies the type of the target path for the template definition.
    /// </summary>
    public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

    /// <summary>
    /// Returns the list of parameters that can be passed to the template.
    /// </summary>
    [DisallowNull]
    public virtual List<Parameter> Parameters { get; } = [];

    /// <summary>
    /// Returns the definition of the template.
    /// </summary>
    [DisallowNull]
    public abstract ConditionedList<T> Definition { get; }

    internal abstract string YamlProperty { get; }

    /// <summary>
    /// Serializes the template into a YAML string.
    /// </summary>
    /// <returns>A YAML string.</returns>
    public string Serialize()
    {
        var template = new ConditionedDictionary();

        if (Parameters != null && Parameters.Count > 0)
        {
            template.Add("parameters", Parameters);
        }

        template.Add(YamlProperty, Definition);

        return SharplinerSerializer.Serialize(template);
    }

    /// <summary>
    /// Returns the list of validations that should be run on the template definition (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    public abstract IReadOnlyCollection<IDefinitionValidation> Validations { get; }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// </summary>
    /// <remarks>
    /// Leave empty array to omit file header.
    /// </remarks>
    public virtual string[]? Header => SharplinerPublisher.GetDefaultHeader(GetType());

    /// <summary>
    /// Disallow any other types than what we define here as AzDO only supports these.
    /// </summary>
    internal TemplateDefinition()
    {
    }
}
