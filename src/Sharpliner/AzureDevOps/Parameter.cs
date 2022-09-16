using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

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
    public string? DisplayName { get; }

    [YamlMember(Order = 110)]
    public abstract string Type { get; }

    internal Parameter(string name, string? displayName = null)
    {
        Name = name;
        DisplayName = displayName;
    }
}

/// <summary>
/// Allows to define which parameters the template expects.
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

    internal Parameter(string name, string? displayName = null, T? defaultValue = default, IEnumerable<T>? allowedValues = null)
        : base(name, displayName)
    {
        Default = defaultValue;
        AllowedValues = allowedValues;
    }
}

public sealed record StringParameter : Parameter<string>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    internal StringParameter(string name, string? displayName = null, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    public override string Type => "string";
}

public sealed record NumberParameter : Parameter<int?>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    internal NumberParameter(string name, string? displayName = null, int? defaultValue = null, IEnumerable<int?>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    public override string Type => "number";
}

public sealed record BooleanParameter : Parameter<bool?>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal BooleanParameter(string name, string? displayName = null, bool? defaultValue = null)
        : base(name, displayName, defaultValue)
    {
    }

    public override string Type => "boolean";
}

public sealed record ObjectParameter : Parameter<ConditionedDictionary>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal ObjectParameter(string name, string? displayName = null, ConditionedDictionary? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "object";
}

public sealed record StepParameter : Parameter<Step>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StepParameter(string name, string? displayName = null, Step? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "step";
}

public sealed record StepListParameter : Parameter<ConditionedList<Step>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StepListParameter(string name, string? displayName = null, ConditionedList<Step>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stepList";
}

public sealed record JobParameter : Parameter<JobBase>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal JobParameter(string name, string? displayName = null, JobBase? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "job";
}

public sealed record JobListParameter : Parameter<ConditionedList<JobBase>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal JobListParameter(string name, string? displayName = null, ConditionedList<JobBase>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "jobList";
}

public sealed record DeploymentParameter : Parameter<DeploymentJob>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal DeploymentParameter(string name, string? displayName = null, DeploymentJob? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "deployment";
}

public sealed record DeploymentListParameter : Parameter<ConditionedList<DeploymentJob>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal DeploymentListParameter(string name, string? displayName = null, ConditionedList<DeploymentJob>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "deploymentList";
}

public sealed record StageParameter : Parameter<Stage>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StageParameter(string name, string? displayName = null, Stage? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stage";
}

public sealed record StageListParameter : Parameter<ConditionedList<Stage>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StageListParameter(string name, string? displayName = null, ConditionedList<Stage>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stageList";
}

internal sealed record StageReference : Stage, IYamlConvertible
{
    private readonly string _parameterName;

    public StageReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}

internal sealed record JobReference : JobBase, IYamlConvertible
{
    private readonly string _parameterName;

    public JobReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}

internal sealed record StepReference : Step, IYamlConvertible
{
    private readonly string _parameterName;

    public StepReference(string parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(new ParameterReference(_parameterName)));
}
