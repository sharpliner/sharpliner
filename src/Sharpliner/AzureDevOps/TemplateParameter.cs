﻿using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract record TemplateParameter
    {
        /// <summary>
        /// Name of the parameter, can be referenced in the template as ${{ parameters.name }}
        /// </summary>
        [YamlMember(Order = 100)]
        public string Name { get; init; }

        [YamlMember(Order = 110)]
        public abstract string Type { get; }

        internal TemplateParameter(string name)
        {
            Name = name;
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

        internal TemplateParameter(string name, T? defaultValue = default, IEnumerable<T>? allowedValues = null) : base(name)
        {
            Default = defaultValue;
            AllowedValues = allowedValues;
        }
    }

    public sealed record StringTemplateParameter : TemplateParameter<string>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        /// <param name="allowedValues">Allowed list of values (for some data types)</param>
        public StringTemplateParameter(string name, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
            : base(name, defaultValue, allowedValues)
        {
        }

        public override string Type => "string";
    }

    public sealed record NumberTemplateParameter : TemplateParameter<int>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        /// <param name="allowedValues">Allowed list of values (for some data types)</param>
        public NumberTemplateParameter(string name, int defaultValue = 0, IEnumerable<int>? allowedValues = null)
            : base(name, defaultValue, allowedValues)
        {
        }

        public override string Type => "number";
    }

    public sealed record BooleanTemplateParameter : TemplateParameter<bool>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public BooleanTemplateParameter(string name, bool defaultValue = false)
            : base(name, defaultValue)
        {
        }

        public override string Type => "boolean";
    }

    public sealed record ObjectTemplateParameter : TemplateParameter<Dictionary<string, object>>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public ObjectTemplateParameter(string name, Dictionary<string, object>? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "object";
    }

    public sealed record StepTemplateParameter : TemplateParameter<Step>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public StepTemplateParameter(string name, Step? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "step";
    }

    public sealed record StepListTemplateParameter : TemplateParameter<ConditionedList<Step>>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public StepListTemplateParameter(string name, ConditionedList<Step>? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "stepList";
    }

    public sealed record JobTemplateParameter : TemplateParameter<Job>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public JobTemplateParameter(string name, Job? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "job";
    }

    public sealed record JobListTemplateParameter : TemplateParameter<ConditionedList<Job>>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public JobListTemplateParameter(string name, ConditionedList<Job>? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "jobList";
    }

    /* TODO: When we have Deployment https://github.com/sharpliner/sharpliner/issues/72
     * 
     * public sealed record DeploymentTemplateParameter : TemplateParameter<>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public DeploymentTemplateParameter(string name, ? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "deployment";
    }

    public sealed record DeploymentListTemplateParameter : TemplateParameter<>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public DeploymentListTemplateParameter(string name, ConditionedList<>? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "deploymentList";
    }*/

    public sealed record StageTemplateParameter : TemplateParameter<Stage>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public StageTemplateParameter(string name, Stage? defaultValue = null)
            : base(name, defaultValue, null)
        {
        }

        public override string Type => "stage";
    }

    public sealed record StageListTemplateParameter : TemplateParameter<ConditionedList<Stage>>
    {
        /// <summary>
        /// Creates a parameter definition
        /// </summary>
        /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
        /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
        public StageListTemplateParameter(string name, ConditionedList<Stage>? defaultValue = null)
            : base(name, defaultValue, null)
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

    internal sealed record JobParameterReference : Job, IYamlConvertible
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

    // TODO: Deployment / deploymentList https://github.com/sharpliner/sharpliner/issues/72
}
