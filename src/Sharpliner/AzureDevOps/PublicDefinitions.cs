// This file contains all definitions that users should override to use Sharpliner.
// To learn more, see https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Validation;
using Sharpliner.Common;

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
    TemplateParameters ToTemplateParameters();
}

public abstract class JobTemplateDefinition<TParameters> : JobTemplateDefinition
    where TParameters : ITemplateParametersProvider, new()
{
    public sealed override List<Parameter> Parameters => GetParameters(TemplateParameters);

    [DisallowNull]
    public TParameters TemplateParameters { get; } = new TParameters();

    private static List<Parameter> GetParameters(TParameters parameters)
    {
        var result = new List<Parameter>();
        foreach (var parameter in typeof(TParameters).GetProperties()
            .Where(IsParameterProperty))
        {
            var paramType = parameter.GetType();
            if (paramType == typeof(string))
            {
                result.Add(new StringParameter(parameter.Name));
            }
            else if (paramType == typeof(bool))
            {
                result.Add(new BooleanParameter(parameter.Name));
            }
            // TODO: Add more types as needed
        }
        return result;

        static bool IsParameterProperty(PropertyInfo property)
        {
            // TODO: Add more types as needed
            return
                property.GetSetMethod()?.IsPublic is true &&
                property.GetGetMethod()?.IsPublic is true &&
                (
                    property.PropertyType == typeof(string)
                    || property.PropertyType == typeof(bool)
                );
        }
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
