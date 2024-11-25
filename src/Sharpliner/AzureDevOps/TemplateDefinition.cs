using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
    /// Defines a string template parameter with a closed set of allowed values based on an enum.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="name">The name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter EnumParameter<TEnum>(string name, TEnum? defaultValue = null)
        where TEnum : struct, Enum
        => new EnumParameter<TEnum>(name, null, defaultValue);

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
    protected static Parameter<Step> StepParameter(string name, Step? defaultValue = null)
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
    protected static Parameter<JobBase> JobParameter(string name, JobBase? defaultValue = null)
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
    protected static Parameter<Stage> StageParameter(string name, Stage? defaultValue = null)
        => new StageParameter(name, null, defaultValue);

    /// <summary>
    /// Defines a stageList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static Parameter StageListParameter(string name, ConditionedList<Stage>? defaultValue = null)
        => new StageListParameter(name, null, defaultValue);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a stage defined in parameters.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to reference</param>
    /// <returns>A stage reference</returns>
    protected static Stage StageParameterReference(string parameterName) => new StageReference(parameterName);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a stage defined in parameters.
    /// </summary>
    /// <param name="parameter">The parameter to reference</param>
    /// <returns>A stage reference</returns>
    protected static Stage StageParameterReference(Parameter<Stage> parameter) => new StageReference(parameter.Name);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a job defined in parameters.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to reference</param>
    /// <returns>A job reference</returns>
    protected static JobBase JobParameterReference(string parameterName) => new JobReference(parameterName);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a job defined in parameters.
    /// </summary>
    /// <param name="parameter">The parameter to reference</param>
    /// <returns>A job reference</returns>
    protected static JobBase JobParameterReference(Parameter<JobBase> parameter) => new JobReference(parameter.Name);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a step defined in parameters.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to reference</param>
    /// <returns>A step reference</returns>
    protected static Step StepParameterReference(string parameterName) => new StepReference(parameterName);

    /// <summary>
    /// Allows the <c>${{ parameters.name }}</c> notation for a step defined in parameters.
    /// </summary>
    /// <param name="parameter">The parameter to reference</param>
    /// <returns>A step reference</returns>
    protected static Step StepParameterReference(Parameter<Step> parameter) => new StepReference(parameter.Name);

    /// <summary>
    /// Utility class that allows the <c>${{ parameters.name }}</c> notation for a parameter.
    /// </summary>
    public sealed class TemplateParameterReference
    {
        /// <summary>
        /// <para>
        /// Allows the <c>${{ parameters.name }}</c> notation for a parameter.
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// parameters["foo"]
        /// </code>
        /// will generate:
        /// <code lang="yaml">
        /// ${{ parameters.foo }}
        /// </code>
        /// </summary>
        public ParameterReference this[string parameterName] => new(parameterName);

        internal TemplateParameterReference()
        {
        }
    }
}

/// <summary>
/// This is the ancestor of all definitions that produce a Azure pipelines template.
/// </summary>
/// <typeparam name="T">Type of the part of the pipeline that this template is for (one of extends, stages, steps, jobs or variables)</typeparam>
public abstract class TemplateDefinitionBase<T> : TemplateDefinition, ISharplinerDefinition
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
    public abstract T Definition { get; }

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

        template.Add(YamlProperty, Definition!);

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
    internal TemplateDefinitionBase()
    {
    }
}

/// <summary>
/// This is the ancestor of all definitions that produce a Azure pipelines template.
/// </summary>
/// <typeparam name="T">Type of the part of the pipeline that this template is for (one of stages, steps, jobs or variables)</typeparam>
public abstract class TemplateDefinition<T> : TemplateDefinitionBase<ConditionedList<T>>
{
    /// <summary>
    /// Disallow any other types than what we define here as AzDO only supports these.
    /// </summary>
    internal TemplateDefinition()
    {
    }
}

/// <summary>
/// This class is a helper for defining templates with typed parameters.
/// </summary>
/// <typeparam name="T">Type of the part of the pipeline that this template is for (one of stages, steps, jobs or variables)</typeparam>
/// <typeparam name="TParameters">Type of the parameters that can be passed to the template</typeparam>
public abstract class TemplateDefinition<T, TParameters> : TemplateDefinition<T> where TParameters : class, new()
{
    private readonly TParameters _typedParameters;

    internal TemplateDefinition(TParameters? typedParameters = null)
    {
        _typedParameters = typedParameters ?? new();
    }

    /// <inheritdoc/>
    public sealed override List<Parameter> Parameters => ToParameters();

    private static List<Parameter> ToParameters()
    {
        var result = new List<Parameter>();
        var defaultParameters = new TParameters();
        foreach (var property in typeof(TParameters).GetProperties())
        {
            var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias ?? CamelCaseNamingConvention.Instance.Apply(property.Name);
            var defaultValue = property.GetValue(defaultParameters);
            var allowedValues = property.GetCustomAttribute<AllowedValuesAttribute>()?.Values;
            Parameter parameter = property.PropertyType switch
            {
                { } type when type.IsEnum => (Parameter)Activator.CreateInstance(typeof(EnumParameter<>).MakeGenericType(type), name, null, defaultValue)!,
                { } type when type == typeof(string) => new StringParameter(name, defaultValue: defaultValue as string, allowedValues: allowedValues?.Cast<string>()),
                { } type when type == typeof(bool?) || type == typeof(bool) => new BooleanParameter(name, defaultValue: defaultValue as bool?),
                { } type when type == typeof(int?) || type == typeof(int) => new NumberParameter(name, defaultValue: defaultValue as int?, allowedValues: allowedValues?.Cast<int?>()),
                { } type when type == typeof(Step) => new StepParameter(name, defaultValue: defaultValue as Step),
                { } type when type == typeof(ConditionedList<Step>) => new StepListParameter(name, defaultValue: defaultValue as ConditionedList<Step>),
                { } type when type.IsAssignableFrom(typeof(JobBase)) => new JobParameter(name, defaultValue: defaultValue as JobBase),
                { } type when type == typeof(ConditionedList<JobBase>) => new JobListParameter(name, defaultValue: defaultValue as ConditionedList<JobBase>),
                { } type when type == typeof(DeploymentJob) => new DeploymentParameter(name, defaultValue: defaultValue as DeploymentJob),
                { } type when type == typeof(ConditionedList<DeploymentJob>) => new DeploymentListParameter(name, defaultValue: defaultValue as ConditionedList<DeploymentJob>),
                { } type when type == typeof(Stage) => new StageParameter(name, defaultValue: defaultValue as Stage),
                { } type when type == typeof(ConditionedList<Stage>) => new StageListParameter(name, defaultValue: defaultValue as ConditionedList<Stage>),
                _ => new ObjectParameter(name),
            };

            result.Add(parameter);
        }

        return result;
    }

    private static TemplateParameters ToTemplateParameters(TParameters parameters)
    {
        var result = new TemplateParameters();
        var defaultParameters = new TParameters();

        foreach (var property in typeof(TParameters).GetProperties())
        {
            var value = property.GetValue(parameters);
            var defaultValue = property.GetValue(defaultParameters);
            if (value is not null && !SharplinerSerializer.Serialize(value!).Equals(SharplinerSerializer.Serialize(defaultValue!)))
            {
                var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias;
                name ??= CamelCaseNamingConvention.Instance.Apply(property.Name);

                result.Add(name, value);
            }
        }

        return result;
    }

    /// <summary>
    /// Implicitly converts a typed template definition to a template.
    /// </summary>
    /// <param name="definition">The typed template definition</param>
    public static implicit operator Template<T>(TemplateDefinition<T, TParameters> definition)
    {
        return new Template<T>(definition.TargetFile, ToTemplateParameters(definition._typedParameters));
    }
}

/// <summary>
/// This class is a helper for defining templates with typed parameters.
/// <para>
/// We are not using the <c>System.ComponentModel.DataAnnotations.AllowedValuesAttribute</c> because it is not available in .NET 6 and .NET 7.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AllowedValuesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllowedValuesAttribute"/> class.
    /// </summary>
    /// <param name="values">
    /// A list of values that the validated value should be equal to.
    /// </param>
    public AllowedValuesAttribute(params object?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        Values = values;
    }

    /// <summary>
    /// Gets the list of values allowed by this attribute.
    /// </summary>
    public object?[] Values { get; }
}
