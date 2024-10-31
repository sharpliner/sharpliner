using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Base class for defining parameters that can be used in templates and pipelines.
/// </summary>
public abstract record Parameter
{
    /// <summary>
    /// Name of the parameter, can be referenced in the template as ${{ parameters.name }}
    /// </summary>
    [YamlMember(Order = 100)]
    public string Name { get; init; }

    /// <summary>
    /// Display name of the parameter (in case this is a pipeline parameter)
    /// </summary>
    [YamlMember(Order = 101)]
    public string? DisplayName { get; init; }

    /// <summary>
    /// The azure-pipeline type of the parameter.
    /// </summary>
    [YamlMember(Order = 110)]
    public abstract string Type { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    public Parameter(string name, string? displayName = null)
    {
        Name = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// Converts a <see cref="Parameter"/> to a <see cref="string"/> representation of the reference to the parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public static implicit operator string(Parameter parameter) => new ParameterReference(parameter.Name);

    /// <summary>
    /// Converts a <see cref="Parameter"/> to a <see cref="IfExpression"/> by getting the reference to the parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public static implicit operator IfExpression(Parameter parameter) => new ParameterReference(parameter.Name);

    /// <summary>
    /// Converts a <see cref="Parameter"/> to a <see cref="InlineExpression"/> by getting the reference to the parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public static implicit operator InlineExpression(Parameter parameter) => new ParameterReference(parameter.Name);
}

/// <summary>
/// Base class for defining parameters that can be used in templates and pipelines.
/// This class is generic and should be used for defining parameters with a specific dotnet data type.
/// </summary>
public abstract record Parameter<T> : Parameter
{
    /// <summary>
    /// Default value; if no default, then the parameter MUST be given by the user at runtime
    /// </summary>
    [YamlMember(Order = 120, DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public T? Default { get; init; }

    /// <summary>
    /// Allowed list of values (for some data types)
    /// </summary>
    [YamlMember(Alias = "values", Order = 130)]
    public IEnumerable<T>? AllowedValues { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    public Parameter(string name, string? displayName = null, T? defaultValue = default, IEnumerable<T>? allowedValues = null)
        : base(name, displayName)
    {
        Default = defaultValue;
        AllowedValues = allowedValues;
    }
}

/// <summary>
/// Class for defining <see cref="string"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record StringParameter : Parameter<string>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of string values.</param>
    public StringParameter(string name, string? displayName = null, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    /// <inheritdoc />
    public override string Type => "string";
}

/// <summary>
/// Class for defining <see cref="int"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record NumberParameter : Parameter<int?>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    public NumberParameter(string name, string? displayName = null, int? defaultValue = null, IEnumerable<int?>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    /// <inheritdoc />
    public override string Type => "number";
}

/// <summary>
/// Class for defining a single <see cref="bool"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record BooleanParameter : Parameter<bool?>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public BooleanParameter(string name, string? displayName = null, bool? defaultValue = null)
        : base(name, displayName, defaultValue)
    {
    }

    /// <inheritdoc />
    public override string Type => "boolean";
}

/// <summary>
/// Class for defining parameters with custom structure that can be used in templates and pipelines.
/// </summary>
public sealed record ObjectParameter : Parameter<ConditionedDictionary>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public ObjectParameter(string name, string? displayName = null, ConditionedDictionary? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "object";
}

/// <summary>
/// Class for defining parameters with custom structure based on a strongly-typed collection that can be used in templates and pipelines.
/// </summary>
public sealed record ObjectParameter<T> : Parameter<ConditionedList<T>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public ObjectParameter(string name, string? displayName = null, ConditionedList<T>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "object";
}

/// <summary>
/// Class for defining a single <c><see cref="Step"/></c> parameter that can be used in templates and pipelines.
/// </summary>
public sealed record StepParameter : Parameter<Step>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public StepParameter(string name, string? displayName = null, Step? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "step";
}

/// <summary>
/// Class for defining a sequence of <see cref="Step"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record StepListParameter : Parameter<ConditionedList<Step>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public StepListParameter(string name, string? displayName = null, ConditionedList<Step>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "stepList";
}

/// <summary>
/// Class for defining a single <c><see cref="Job"/></c> parameter that can be used in templates and pipelines.
/// </summary>
public sealed record JobParameter : Parameter<JobBase>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public JobParameter(string name, string? displayName = null, JobBase? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "job";
}

/// <summary>
/// Class for defining a sequence of <see cref="Job"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record JobListParameter : Parameter<ConditionedList<JobBase>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public JobListParameter(string name, string? displayName = null, ConditionedList<JobBase>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "jobList";
}

/// <summary>
/// Class for defining a single <c><see cref="DeploymentJob"/></c> parameter that can be used in templates and pipelines.
/// </summary>
public sealed record DeploymentParameter : Parameter<DeploymentJob>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public DeploymentParameter(string name, string? displayName = null, DeploymentJob? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "deployment";
}

/// <summary>
/// Class for defining a sequence of <see cref="DeploymentJob"/> parameters that can be used in templates and pipelines.
/// </summary>
public sealed record DeploymentListParameter : Parameter<ConditionedList<DeploymentJob>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public DeploymentListParameter(string name, string? displayName = null, ConditionedList<DeploymentJob>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "deploymentList";
}

/// <summary>
/// Class for defining a single <c><see cref="Stage"/></c> parameter that can be used in templates and pipelines.
/// </summary>
public sealed record StageParameter : Parameter<Stage>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public StageParameter(string name, string? displayName = null, Stage? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "stage";
}

/// <summary>
/// Class for defining of sequence of <see cref="Stage"/> parameters  that can be used in templates and pipelines.
/// </summary>
public sealed record StageListParameter : Parameter<ConditionedList<Stage>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as <c>${{ parameters.name }}</c></param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    public StageListParameter(string name, string? displayName = null, ConditionedList<Stage>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    /// <inheritdoc />
    public override string Type => "stageList";
}

internal sealed record StageReference : Stage, IYamlConvertible
{
    private readonly string _parameterName;

    public StageReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}

internal sealed record JobReference : JobBase, IYamlConvertible
{
    private readonly string _parameterName;

    public JobReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}

internal sealed record StepReference : Step, IYamlConvertible
{
    private readonly string _parameterName;

    public StepReference(string parameterName)
    {
        _parameterName = parameterName;
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}
