// This file contains all definitions that users should override to use Sharpliner.
// To learn more, see https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;
using Sharpliner.AzureDevOps.Validation;
using Sharpliner.Common;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner.AzureDevOps;

#region Pipelines - override these to define pipelines

/// <summary>
/// Inherit from this class to define a full Azure DevOps pipeline.
/// For a pipeline with only a single stage, consider using SingleStagePipelineDefinition.
/// </summary>
public abstract class PipelineDefinition : PipelineDefinitionBase<Pipeline>
{
}

/// <summary>
/// Inherit from this class to define a pipeline with a single stage where you only define jobs.
/// </summary>
public abstract class SingleStagePipelineDefinition : PipelineDefinitionBase<SingleStagePipeline>
{
}


/// <summary>
/// Inherit from this class to define a pipeline that extends a template.
/// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/extends?view=azure-pipelines
/// </summary>
public abstract class ExtendsPipelineDefinition : ExtendsPipelineDefinition<PipelineWithExtends>
{
}

/// <summary>
/// Inherit from this class to define a pipeline that extends a template.
/// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/extends?view=azure-pipelines
/// </summary>
public abstract class ExtendsPipelineDefinition<TPipeline> : PipelineDefinitionBase<TPipeline>
    where TPipeline : PipelineWithExtends
{
}

#endregion

#region Pipeline templates - override these to define pipeline templates

/// <summary>
/// Inherit from this class to define a stage template.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class StageTemplateDefinition : TemplateDefinition<Stage>
{
    internal sealed override string YamlProperty => "stages";

    /// <inheritdoc/>
    public sealed override IReadOnlyCollection<IDefinitionValidation> Validations => Definition.GetStageValidations();
}

/// <summary>
/// Inherit from this class to define a job template.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class JobTemplateDefinition : TemplateDefinition<JobBase>
{
    internal sealed override string YamlProperty => "jobs";

    /// <inheritdoc/>
    public sealed override IReadOnlyCollection<IDefinitionValidation> Validations => Definition.GetJobValidations();
}

public interface ITemplateParametersProvider
{
    List<Parameter> ToParameters();

    TemplateParameters ToTemplateParameters();
}

public abstract class TemplateParametersProviderBase<TParameters> : ITemplateParametersProvider where TParameters : class, new()
{
    public List<Parameter> ToParameters()
    {
        var result = new List<Parameter>();
        var defaultParameters = new TParameters();
        foreach (var property in typeof(TParameters).GetProperties())
        {
            var name = property.Name;
            var yamlMember = property.GetCustomAttribute<YamlMemberAttribute>();
            if (yamlMember?.Alias is not null)
            {
                name = yamlMember.Alias;
            }

            var defaultValue = property.GetValue(defaultParameters);
            Parameter parameter;
            if (property.PropertyType.IsEnum)
            {
                parameter = new StringParameter(name, defaultValue: defaultValue as string, allowedValues: Enum.GetNames(property.PropertyType));
            }
            else if (property.PropertyType == typeof(string))
            {
                parameter = new StringParameter(name, defaultValue: defaultValue as string);
            }
            else if (property.PropertyType == typeof(bool))
            {
                parameter = new BooleanParameter(name, defaultValue: defaultValue as bool?);
            }
            else if (property.PropertyType == typeof(int))
            {
                parameter = new NumberParameter(name, defaultValue: defaultValue as int?);
            }
            else if (property.PropertyType == typeof(Step))
            {
                parameter = new StepParameter(name, defaultValue: defaultValue as Step);
            }
            else if (property.PropertyType == typeof(ConditionedList<Step>))
            {
                parameter = new StepListParameter(name, defaultValue: defaultValue as ConditionedList<Step>);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(JobBase)))
            {
                parameter = new JobParameter(name, defaultValue: defaultValue as JobBase);
            }
            else if (property.PropertyType == typeof(ConditionedList<JobBase>))
            {
                parameter = new JobListParameter(name, defaultValue: defaultValue as ConditionedList<JobBase>);
            }
            else if (property.PropertyType == typeof(DeploymentJob))
            {
                parameter = new DeploymentParameter(name, defaultValue: defaultValue as DeploymentJob);
            }
            else if (property.PropertyType == typeof(ConditionedList<DeploymentJob>))
            {
                parameter = new DeploymentListParameter(name, defaultValue: defaultValue as ConditionedList<DeploymentJob>);
            }
            else if (property.PropertyType == typeof(Stage))
            {
                parameter = new StageParameter(name, defaultValue: defaultValue as Stage);
            }
            else if (property.PropertyType == typeof(ConditionedList<Stage>))
            {
                parameter = new StageListParameter(name, defaultValue: defaultValue as ConditionedList<Stage>);
            }
            else
            {
                // TODO: use reflection to create an instance of ObjectParameter<T>
                parameter = new ObjectParameter(name);
            }

            result.Add(parameter);
        }

        return result;
    }

    public TemplateParameters ToTemplateParameters()
    {
        var result = new TemplateParameters();
        var defaultParameters = new TParameters();

        foreach (var property in typeof(TParameters).GetProperties())
        {
            var value = property.GetValue(this);
            if (value is not null && value != property.GetValue(defaultParameters))
            {
                var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias;
                name ??= CamelCaseNamingConvention.Instance.Apply(property.Name);

                result.Add(name, value);
            }
        }

        return result;
    }
}

public abstract class JobTemplateDefinition<TParameters> : JobTemplateDefinition
    where TParameters : ITemplateParametersProvider, new()
{
    public sealed override List<Parameter> Parameters => TemplateParameters.ToParameters();

    [DisallowNull]
    public TParameters TemplateParameters { get; } = new TParameters();
}

public abstract class StageTemplateDefinition<TParameters> : StageTemplateDefinition
    where TParameters : ITemplateParametersProvider, new()
{
    public sealed override List<Parameter> Parameters => TemplateParameters.ToParameters();

    [DisallowNull]
    public TParameters TemplateParameters { get; } = new TParameters();


    // TODO: extract the expression property path correctly
    protected static InlineCondition Equal(object parameterRef, InlineExpression expression2, [CallerArgumentExpression(nameof(parameterRef))]string parameterExpression = "")
        => AzureDevOpsDefinition.Equal(new ParameterReference(NormalizeParameterExpression(parameterExpression)), expression2);

    protected static string NormalizeParameterExpression(string parameterExpression)
    {
        parameterExpression = parameterExpression.Substring("TemplateParameters.".Length);

        var type = typeof(TParameters);
        var normalizeExpression = new List<string>();

        var parts = parameterExpression.Split('.');
        Console.WriteLine($"parts='{parts}', parameterExpression='{parameterExpression}'");
        foreach (var part in parts)
        {
            var property = type.GetProperty(part);
            if (property is null)
            {
                throw new ArgumentNullException($"Cannot get '{part}' property from type '{type.Name}'");
            }

            var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias
                ?? CamelCaseNamingConvention.Instance.Apply(property.Name);
            normalizeExpression.Add(name);

            type = property.PropertyType;
        }

        return string.Join('.', normalizeExpression);
    }
}

/// <summary>
/// Inherit from this class to define a step template.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class StepTemplateDefinition : TemplateDefinition<Step>
{
    internal sealed override string YamlProperty => "steps";

    /// <inheritdoc/>
    public sealed override IReadOnlyCollection<IDefinitionValidation> Validations => [];
}

/// <summary>
/// Inherit from this class to define a variable template.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class VariableTemplateDefinition : TemplateDefinition<VariableBase>
{
    internal sealed override string YamlProperty => "variables";

    /// <inheritdoc/>
    public sealed override IReadOnlyCollection<IDefinitionValidation> Validations => [];
}

#endregion

#region Definition collections - use these to generate definitions dynamically

/// <summary>
/// Inherit from this class when you want to dynamically generate multiple pipelines
/// For a pipeline with only a single stage, consider using SingleStagePipelineDefinitions
/// </summary>
public abstract class PipelineCollection : PipelineDefinitionCollection<Pipeline>
{
}

/// <summary>
/// Inherit from this class to dynamically generate multiple pipelines with a single stage
/// </summary>
public abstract class SingleStagePipelineCollection : PipelineDefinitionCollection<SingleStagePipeline>
{
}

/// <summary>
/// Inherit from this class when you want to dynamically generate multiple stage templates
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class StageTemplateCollection : TemplateDefinitionCollection<Stage>
{
    internal sealed override string YamlProperty => "stages";

    internal sealed override IReadOnlyCollection<IDefinitionValidation> GetValidations(TemplateDefinitionData<Stage> definition)
        => definition.Definition.GetStageValidations();
}

/// <summary>
/// Inherit from this class when you want to dynamically generate multiple job templates
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class JobTemplateCollection : TemplateDefinitionCollection<JobBase>
{
    internal sealed override string YamlProperty => "jobs";

    internal sealed override IReadOnlyCollection<IDefinitionValidation> GetValidations(TemplateDefinitionData<JobBase> definition)
        => definition.Definition.GetJobValidations();
}

/// <summary>
/// Inherit from this class when you want to dynamically generate multiple step templates
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class StepTemplateCollection : TemplateDefinitionCollection<Step>
{
    internal sealed override string YamlProperty => "steps";

    internal sealed override IReadOnlyCollection<IDefinitionValidation> GetValidations(TemplateDefinitionData<Step> definition)
        => [];
}

/// <summary>
/// Inherit from this class when you want to dynamically generate multiple step templates
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#template-references">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract class VariableTemplateCollection : TemplateDefinitionCollection<VariableBase>
{
    internal sealed override string YamlProperty => "variables";

    internal sealed override IReadOnlyCollection<IDefinitionValidation> GetValidations(TemplateDefinitionData<VariableBase> definition)
        => [];
}

#endregion

#region Definition libraries - override these to create sets of reusable parts (like templates but in C# only)

/// <summary>
/// Inherit from this class to create a reusable set of stages.
/// This library can then be inserted to multiple places.
/// </summary>
public abstract class StageLibrary : DefinitionLibrary<Stage>
{
    internal override IEnumerable<Conditioned<Stage>> Items => Stages;

    /// <summary>
    /// The list of stages in this library.
    /// </summary>
    public abstract List<Conditioned<Stage>> Stages { get; }
}

/// <summary>
/// Inherit from this class to create a reusable set of build jobs.
/// This library can then be inserted to multiple places.
/// </summary>
public abstract class JobLibrary : DefinitionLibrary<JobBase>
{
    internal override IEnumerable<Conditioned<JobBase>> Items => Jobs;

    /// <summary>
    /// The list of jobs in this library.
    /// </summary>
    public abstract List<Conditioned<JobBase>> Jobs { get; }
}

/// <summary>
/// Inherit from this class to create a reusable set of build steps.
/// This library can then be inserted to multiple places.
/// </summary>
public abstract class StepLibrary : DefinitionLibrary<Step>
{
    internal override IEnumerable<Conditioned<Step>> Items => Steps;

    /// <summary>
    /// The list of steps in this library.
    /// </summary>
    public abstract List<Conditioned<Step>> Steps { get; }
}

/// <summary>
/// Inherit from this class to create a reusable set of variable definitions.
/// This library can then be inserted to multiple places.
/// </summary>
public abstract class VariableLibrary : DefinitionLibrary<VariableBase>
{
    internal override IEnumerable<Conditioned<VariableBase>> Items => Variables;

    /// <summary>
    /// The list of variables in this library.
    /// </summary>
    public abstract List<Conditioned<VariableBase>> Variables { get; }
}

#endregion
