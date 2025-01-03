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
    public sealed override List<Parameter> Parameters => TypedTemplateUtils<TParameters>.ToParameters();


    /// <summary>
    /// Implicitly converts a typed template definition to a template.
    /// </summary>
    /// <param name="definition">The typed template definition</param>
    public static implicit operator Template<T>(TemplateDefinition<T, TParameters> definition)
    {
        return new Template<T>(definition.TargetFile, TypedTemplateUtils<TParameters>.ToTemplateParameters(definition._typedParameters));
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
