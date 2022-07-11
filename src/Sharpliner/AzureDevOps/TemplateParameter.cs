using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

public abstract record TemplateParameter
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

    internal TemplateParameter(string name, string? displayName)
    {
        Name = name;
        DisplayName = displayName;
    }
}

/// <summary>
/// Allows to define which parameters the template expects.
/// </summary>
public abstract record TemplateParameter<T> : TemplateParameter
{
    /// <summary>
    /// Default value; if no default, then the parameter MUST be given by the user at runtime
    /// </summary>
    [YamlMember(Order = 120)]
    public T? Default { get; init; }

    /// <summary>
    /// Allowed list of values (for some data types)
    /// </summary>
    [YamlMember(Alias = "values", Order = 130)]
    public IEnumerable<T>? AllowedValues { get; init; }

    internal TemplateParameter(string name, string? displayName, T? defaultValue = default, IEnumerable<T>? allowedValues = null)
        : base(name, displayName)
    {
        Default = defaultValue;
        AllowedValues = allowedValues;
    }
}

public sealed record StringTemplateParameter : TemplateParameter<string>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    internal StringTemplateParameter(string name, string? displayName, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    public override string Type => "string";
}

public sealed record NumberTemplateParameter : TemplateParameter<int>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    internal NumberTemplateParameter(string name, string? displayName, int defaultValue = 0, IEnumerable<int>? allowedValues = null)
        : base(name, displayName, defaultValue, allowedValues)
    {
    }

    public override string Type => "number";
}

public sealed record BooleanTemplateParameter : TemplateParameter<bool>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal BooleanTemplateParameter(string name, string? displayName, bool defaultValue = false)
        : base(name, displayName, defaultValue)
    {
    }

    public override string Type => "boolean";
}

public sealed record ObjectTemplateParameter : TemplateParameter<ConditionedDictionary>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal ObjectTemplateParameter(string name, string? displayName, ConditionedDictionary? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "object";
}

public sealed record StepTemplateParameter : TemplateParameter<Step>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StepTemplateParameter(string name, string? displayName, Step? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "step";
}

public sealed record StepListTemplateParameter : TemplateParameter<ConditionedList<Step>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StepListTemplateParameter(string name, string? displayName, ConditionedList<Step>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stepList";
}

public sealed record JobTemplateParameter : TemplateParameter<JobBase>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal JobTemplateParameter(string name, string? displayName, JobBase? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "job";
}

public sealed record JobListTemplateParameter : TemplateParameter<ConditionedList<JobBase>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal JobListTemplateParameter(string name, string? displayName, ConditionedList<JobBase>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "jobList";
}

public sealed record DeploymentTemplateParameter : TemplateParameter<DeploymentJob>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal DeploymentTemplateParameter(string name, string? displayName, DeploymentJob? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "deployment";
}

public sealed record DeploymentListTemplateParameter : TemplateParameter<ConditionedList<DeploymentJob>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal DeploymentListTemplateParameter(string name, string? displayName, ConditionedList<DeploymentJob>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "deploymentList";
}

public sealed record StageTemplateParameter : TemplateParameter<Stage>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StageTemplateParameter(string name, string? displayName, Stage? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stage";
}

public sealed record StageListTemplateParameter : TemplateParameter<ConditionedList<Stage>>
{
    /// <summary>
    /// Define a template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter in case this is a pipeline parameter</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    internal StageListTemplateParameter(string name, string? displayName, ConditionedList<Stage>? defaultValue = null)
        : base(name, displayName, defaultValue, null)
    {
    }

    public override string Type => "stageList";
}

internal sealed record StageParameterReference : Stage, IYamlConvertible
{
    private readonly string _parameterName;

    public StageParameterReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar("${{ parameters." + _parameterName + " }}"));
}

internal sealed record JobParameterReference : JobBase, IYamlConvertible
{
    private readonly string _parameterName;

    public JobParameterReference(string parameterName) : base(parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar("${{ parameters." + _parameterName + " }}"));
}

internal sealed record StepParameterReference : Step, IYamlConvertible
{
    private readonly string _parameterName;

    public StepParameterReference(string parameterName)
    {
        _parameterName = parameterName;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar("${{ parameters." + _parameterName + " }}"));
}
