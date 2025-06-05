using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;
using Sharpliner.AzureDevOps.Tasks;
using static Sharpliner.AzureDevOps.TemplateDefinition;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This is a common ancestor for AzDO related definitions (pipelines, templates..) containing useful macros.
/// </summary>
public abstract class AzureDevOpsDefinition
{
    #region Template references

    /// <summary>
    /// Reference a YAML template.
    /// </summary>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    protected static Template<VariableBase> VariableTemplate(string path, TemplateParameters? parameters = null)
        => new(path, parameters);

    /// <summary>
    /// Reference a YAML template.
    /// </summary>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    protected static Template<Stage> StageTemplate(string path, TemplateParameters? parameters = null)
        => new(path, parameters);

    /// <summary>
    /// Reference a YAML template.
    /// </summary>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    protected static Template<JobBase> JobTemplate(string path, TemplateParameters? parameters = null)
        => new(path, parameters);

    /// <summary>
    /// Reference a YAML template.
    /// </summary>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    protected static Template<Step> StepTemplate(string path, TemplateParameters? parameters = null)
        => new(path, parameters);

    #endregion

    #region Library references

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> StageLibrary(StageLibrary library)
        => new LibraryReference<Stage>(library);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static AdoExpression<JobBase> JobLibrary(JobLibrary library)
        => new LibraryReference<JobBase>(library);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> StepLibrary(StepLibrary library)
        => new LibraryReference<Step>(library);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static AdoExpression<VariableBase> VariableLibrary(VariableLibrary library)
        => new LibraryReference<VariableBase>(library);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> ExpandStages(params AdoExpression<Stage>[] stages)
        => new LibraryReference<Stage>(stages);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static AdoExpression<Job> ExpandJobs(params AdoExpression<Job>[] jobs)
        => new LibraryReference<Job>(jobs);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> ExpandSteps(params AdoExpression<Step>[] steps)
        => new LibraryReference<Step>(steps);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static AdoExpression<VariableBase> ExpandVariables(params AdoExpression<VariableBase>[] variables)
        => new LibraryReference<VariableBase>(variables);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> ExpandStages(IEnumerable<AdoExpression<Stage>> stages)
        => new LibraryReference<Stage>(stages);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static AdoExpression<Job> ExpandJobs(IEnumerable<AdoExpression<Job>> jobs)
        => new LibraryReference<Job>(jobs);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> ExpandSteps(IEnumerable<AdoExpression<Step>> steps)
        => new LibraryReference<Step>(steps);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static AdoExpression<VariableBase> ExpandVariables(IEnumerable<AdoExpression<VariableBase>> variables)
        => new LibraryReference<VariableBase>(variables);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> ExpandStages(params Stage[] stages)
        => ExpandStages(stages.Select(x => new AdoExpression<Stage>(x)));

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static AdoExpression<Job> ExpandJobs(params Job[] jobs)
        => ExpandJobs(jobs.Select(x => new AdoExpression<Job>(x)));

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> ExpandSteps(params Step[] steps)
        => ExpandSteps(steps.Select(x => new AdoExpression<Step>(x)));

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static AdoExpression<VariableBase> ExpandVariables(params VariableBase[] variables)
        => ExpandVariables(variables.Select(x => new AdoExpression<VariableBase>(x)));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> ExpandStages(IEnumerable<Stage> stages)
        => ExpandStages(stages.ToArray());

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static AdoExpression<Job> ExpandJobs(IEnumerable<Job> jobs)
        => ExpandJobs(jobs.ToArray());

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> ExpandSteps(IEnumerable<Step> steps)
        => ExpandSteps(steps.ToArray());

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static AdoExpression<VariableBase> ExpandVariables(IEnumerable<VariableBase> variables)
        => ExpandVariables(variables.ToArray());

    /// <summary>
    /// Reference a stage library (series of library stages).
    /// </summary>
    protected static AdoExpression<Stage> StageLibrary<T>() where T : StageLibrary, new()
        => CreateLibraryRef<T, Stage>();

    /// <summary>
    /// Reference a job library (series of library jobs).
    /// </summary>
    protected static AdoExpression<JobBase> JobLibrary<T>() where T : JobLibrary, new()
        => CreateLibraryRef<T, JobBase>();

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static AdoExpression<Step> StepLibrary<T>() where T : StepLibrary, new()
        => CreateLibraryRef<T, Step>();

    /// <summary>
    /// Reference a variable library (set of variable definition).
    /// </summary>
    protected static AdoExpression<VariableBase> VariableLibrary<T>() where T : VariableLibrary, new()
        => CreateLibraryRef<T, VariableBase>();

    /// <summary>
    /// Helper method to create instances of LibraryReference.
    /// </summary>
    /// <typeparam name="TLibrary">User's implementation type of the library</typeparam>
    /// <typeparam name="TDefinition">Definition type (Stage/Job/Step/Variable)</typeparam>
    internal static LibraryReference<TDefinition> CreateLibraryRef<TLibrary, TDefinition>()
        where TLibrary : DefinitionLibrary<TDefinition>, new()
        => new(new TLibrary());

    #endregion

    #region Pipeline variable shorthands

    /// <summary>
    /// Allows the variables[""] notation for conditional definitions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Should not be capitalized to follow YAML syntax")]
    protected static readonly VariablesReference variables = new();

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static AdoExpression<VariableBase> Variable(string name, string value) => new(new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static AdoExpression<VariableBase> Variable(string name, int value) => new(new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static AdoExpression<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static AdoExpression<VariableBase> Variable(string name, Enum value) => new(new Variable(name, value));

    /// <summary>
    /// References a variable group.
    /// </summary>
    /// <param name="name">Group name</param>
    protected static AdoExpression<VariableBase> Group(string name) => new(new VariableGroup(name));

    #endregion

    #region Pipeline task shorthands

    /// <summary>
    /// Creates a bash task.
    /// </summary>
    protected static BashTaskBuilder Bash { get; } = new();

    /// <summary>
    /// Creates a script task.
    /// </summary>
    protected static ScriptTaskBuilder Script { get; } = new();

    /// <summary>
    /// Creates a powershell task.
    /// </summary>
    protected static PowershellTaskBuilder Powershell { get; } = new();

    /// <summary>
    /// Creates a pwsh task.
    /// </summary>
    protected static PwshTaskBuilder Pwsh { get; } = new();

    /// <summary>
    /// Creates a publish task.
    /// </summary>
    protected static PublishTaskBuilder Publish { get; } = new();

    /// <summary>
    /// Creates a checkout task.
    /// </summary>
    protected static CheckoutTaskBuilder Checkout { get; } = new();

    /// <summary>
    /// Creates a download task.
    /// </summary>
    protected static DownloadTaskBuilder Download { get; } = new();

    /// <summary>
    /// Creates an Azure CLI task
    /// </summary>
    protected static AzureCliTaskBuilder AzureCli { get; } = new();

    /// <summary>
    /// Creates a generic pipeline task.
    /// </summary>
    protected static AzureDevOpsTask Task(string taskName, string? displayName = null)
    {
        var task = new AzureDevOpsTask(taskName);

        if (displayName != null)
        {
            task = task with { DisplayName = displayName };
        }

        return task;
    }

    /// <summary>
    /// Creates an UseDotNet or DotNetCoreCLI task.
    /// </summary>
    protected static DotNetTaskBuilder DotNet { get; } = new();

    /// <summary>
    /// This task verifies that you didn't forget to check in your YAML pipeline changes.
    /// </summary>
    /// <param name="pipelineProject">Path to the .csproj where pipelines are defined</param>
    protected static Step ValidateYamlsArePublished(string pipelineProject)
        => DotNet.Build(pipelineProject) with
        {
            DisplayName = "Validate YAML has been published",
            Arguments = $"-p:{nameof(PublishDefinitions.FailIfChanged)}=true"
        };

    /// <summary>
    /// Creates a NuGet task.
    /// </summary>
    protected static NuGetTaskBuilder NuGet { get; } = new();

    #endregion

    #region Pipeline member shorthands

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    protected static Stage Stage(string stageName, string? displayName = null) => new(stageName, displayName);

    /// <summary>
    /// Creates a new job.
    /// </summary>
    protected static Job Job(string jobName, string? displayName = null) => new(jobName, displayName);

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    protected static DeploymentJob DeploymentJob(string jobName, string? displayName = null) => new(jobName, displayName);

    #endregion

    #region Pipeline parameter shorthands

    /// <summary>
    /// <para>
    /// Allows the <c>${{ parameters.name }}</c> notation for parameter reference.
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Should not be capitalized to follow YAML syntax")]
    protected static readonly TemplateParameterReference parameters = new();

    /// <summary>
    /// Defines a string template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    protected static StringParameter StringParameter(string name, string? displayName = null, string? defaultValue = null, IEnumerable<string>? allowedValues = null)
        => new(name, displayName, defaultValue, allowedValues);

    /// <summary>
    /// Defines a string template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static EnumParameter<TEnum> EnumParameter<TEnum>(string name, string? displayName = null, TEnum? defaultValue = null)
        where TEnum : struct, Enum
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a number template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    /// <param name="allowedValues">Allowed list of values (for some data types)</param>
    protected static NumberParameter NumberParameter(string name, string? displayName = null, int? defaultValue = null, IEnumerable<int?>? allowedValues = null)
        => new(name, displayName, defaultValue, allowedValues);

    /// <summary>
    /// Defines a boolean template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static BooleanParameter BooleanParameter(string name, string? displayName = null, bool? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a object template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static ObjectParameter ObjectParameter(string name, string? displayName = null, DictionaryExpression? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a object template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static ObjectParameter<T> ObjectParameter<T>(string name, string? displayName = null, AdoExpressionList<T>? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a step template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static StepParameter StepParameter(string name, string? displayName = null, Step? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a stepList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static StepListParameter StepListParameter(string name, string? displayName = null, AdoExpressionList<Step>? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static JobParameter JobParameter(string name, string? displayName = null, JobBase? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a jobList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static JobListParameter JobListParameter(string name, string? displayName = null, AdoExpressionList<JobBase>? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a deployment job template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static DeploymentParameter DeploymentParameter(string name, string? displayName = null, DeploymentJob? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a deploymentList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static DeploymentListParameter DeploymentListParameter(string name, string? displayName = null, AdoExpressionList<DeploymentJob>? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a stage template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static StageParameter StageParameter(string name, string? displayName = null, Stage? defaultValue = null)
        => new(name, displayName, defaultValue);

    /// <summary>
    /// Defines a stageList template parameter
    /// </summary>
    /// <param name="name">Name of the parameter, can be referenced in the template as ${{ parameters.name }}</param>
    /// <param name="displayName">Display name of the parameter shown in the UI when creating pipeline run</param>
    /// <param name="defaultValue">Default value; if no default, then the parameter MUST be given by the user at runtime</param>
    protected static StageListParameter StageListParameter(string name, string? displayName = null, AdoExpressionList<Stage>? defaultValue = null)
        => new(name, displayName, defaultValue);

    #endregion

    #region Dependency output variables shorthands

    /// <summary>
    /// <para>
    /// Generates the yaml notation to reference dependency output variables within pipelines.  Can be used to generate yaml for dependency variables for use in either stage
    /// entries or job entries.  Note that if the reference was declared within a Deploy job, you must also specify that using <c>.deploy</c>.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// dependencies.job.deploy["stage", "job", "step", "variable"]
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// ${{ dependencies.stage.outputs['job.job.step.variable'] }}
    /// </code>
    /// Which can be used in a pipeline job to reference the variable.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Should not be capitalized to follow YAML syntax")]
    protected static readonly VariableReferences.DependencyVariable dependencies = new();

    #endregion

    #region Conditions, each expressions, ..

    /// <summary>
    /// Start an <c>${{ if () }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// If.NotIn("'$(Environment)'", "'prod'")
    ///     .Group("dev")
    ///     .Group("staging")
    /// .Else
    ///     .Group("prod")
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ if notIn('$(Environment)', 'prod') }}:
    ///   - group: dev
    ///   - group: staging
    /// - ${{ else }}:
    ///   - group: prod
    /// </code>
    /// </summary>
    protected static IfConditionBuilder If => new(null, false);

    /// <summary>
    /// Start an <c>${{ else () }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// DotNet.Pack("ProjectFile") with
    /// {
    ///     Inputs = new()
    ///     {
    ///         {
    ///             If.Equal(parameters["IncludeSymbols"], "true"), new TaskInputs()
    ///             {
    ///                 ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg"
    ///             }
    ///         },
    ///         {
    ///             Else, new TaskInputs()
    ///             {
    ///                 ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build"
    ///             }
    ///         }
    ///     },
    /// },
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: pack
    ///     packagesToPack: ProjectFile
    ///     ${{ if eq(parameters.IncludeSymbols, true) }}:
    ///       arguments: --configuration $(BuildConfiguration) --no-restore --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
    ///     ${{ else }}:
    ///       arguments: --configuration $(BuildConfiguration) --no-restore --no-build
    /// </code>
    /// </summary>
    protected static ElseCondition Else => new();

    /// <summary>
    /// Use this to specify any custom condition (in case you miss some operator or expression).
    /// </summary>
    protected static InlineCondition Condition(string condition) => new InlineCustomCondition(condition);

    /// <summary>
    /// Utility that creates an <c>${{ and(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with AND</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    protected static InlineCondition<T> And<T>(params string[] expressions) => new InlineAndCondition<T>(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ or(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    protected static InlineCondition Or<T>(params string[] expressions) => new InlineOrCondition<T>(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ xor(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    protected static InlineCondition Xor<T>(string expression1, string expression2) => new InlineXorCondition<T>(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ eq(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with AND</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    protected static InlineCondition And<T>(params InlineCondition[] expressions) => new InlineAndCondition<T>(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ or(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    protected static InlineCondition Or<T>(params InlineCondition[] expressions) => new InlineOrCondition<T>(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ xor(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    protected static InlineCondition Xor<T>(Condition expression1, Condition expression2) => new InlineXorCondition<T>(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ eq(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    protected static InlineCondition Equal<T>(InlineExpression expression1, InlineExpression expression2) => new InlineEqualityCondition<T>(true, expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ ne(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    protected static InlineCondition NotEqual<T>(InlineExpression expression1, InlineExpression expression2) => new InlineEqualityCondition<T>(false, expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ contains(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>A <c>contains</c> condition with the specified expressions.</returns>
    protected static InlineCondition Contains<T>(InlineExpression needle, InlineExpression haystack) => new InlineContainsCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ startsWith(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for at start</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>A <c>startsWith</c> condition with the specified expressions.</returns>
    protected static InlineCondition StartsWith<T>(InlineExpression needle, InlineExpression haystack) => new InlineStartsWithCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ endsWith(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for at end</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>endsWith</c> condition with the specified expressions.</returns>
    protected static InlineCondition EndsWith<T>(InlineExpression needle, InlineExpression haystack) => new InlineEndsWithCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ containsValue(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>A <c>containsValue</c> condition with the specified expressions.</returns>
    protected static InlineCondition ContainsValue<T>(InlineExpression needle, params InlineExpression[] haystack) => new InlineContainsValueCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ in(needle, ...haystack) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>in</c> condition with the specified expressions.</returns>
    protected static InlineCondition In<T>(InlineExpression needle, params InlineExpression[] haystack) => new InlineInCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ notIn(needle, ...haystack) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>An <c>notIn</c> condition with the specified expressions.</returns>
    protected static InlineCondition NotIn<T>(InlineExpression needle, params InlineExpression[] haystack) => new InlineNotInCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ gt(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>A <c>gt</c> condition with the specified expressions.</returns>
    protected static InlineCondition Greater<T>(InlineExpression expression1, InlineExpression expression2) => new InlineGreaterCondition<T>(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ lt(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <typeparam name="T">Type of the node to use the conditions on</typeparam>
    /// <returns>A <c>lt</c> condition with the specified expressions.</returns>
    protected static InlineCondition Less<T>(InlineExpression expression1, InlineExpression expression2) => new InlineLessCondition<T>(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ and(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with AND</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    protected static InlineCondition And(params string[] expressions) => new InlineAndCondition(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ or(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    protected static InlineCondition Or(params string[] expressions) => new InlineOrCondition(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ xor(condition1, condition2) }}</c> section.
    /// </summary>
    /// <param name="condition1">First condition</param>
    /// <param name="condition2">Second condition</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    protected static InlineCondition Xor(string condition1, string condition2) => new InlineXorCondition(condition1, condition2);

    /// <summary>
    /// Utility that creates an <c>${{ and(...expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with AND</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    protected static InlineCondition And(params InlineCondition[] expressions) => new InlineAndCondition(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ or(expressions) }}</c> section.
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    protected static InlineCondition Or(params InlineCondition[] expressions) => new InlineOrCondition(expressions);

    /// <summary>
    /// Utility that creates an <c>${{ xor(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    protected static InlineCondition Xor(InlineCondition expression1, InlineCondition expression2) => new InlineXorCondition(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ contains(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>contains</c> condition with the specified expressions.</returns>
    protected static InlineCondition Contains(InlineExpression needle, InlineExpression haystack) => new InlineContainsCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ startsWith(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for at start</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>startsWith</c> condition with the specified expressions.</returns>
    protected static InlineCondition StartsWith(InlineExpression needle, InlineExpression haystack) => new InlineStartsWithCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ endsWith(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle to search for at end</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>An <c>endsWith</c> condition with the specified expressions.</returns>
    protected static InlineCondition EndsWith(InlineExpression needle, InlineExpression haystack) => new InlineEndsWithCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ in(needle, ...haystack) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <returns>An <c>in</c> condition with the specified expressions.</returns>
    protected static InlineCondition In(InlineExpression needle, params InlineExpression[] haystack) => new InlineInCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ notIn(needle, ...haystack) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <returns>An <c>notIn</c> condition with the specified expressions.</returns>
    protected static InlineCondition NotIn(InlineExpression needle, params InlineExpression[] haystack) => new InlineNotInCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ containsValue(haystack, needle) }}</c> section.
    /// </summary>
    /// <param name="needle">Needle item to search for</param>
    /// <param name="haystack">Haystack of items to search in</param>
    /// <returns>A <c>containsValue</c> condition with the specified expressions.</returns>
    protected static InlineCondition ContainsValue(InlineExpression needle, params InlineExpression[] haystack) => new InlineContainsValueCondition(needle, haystack);

    /// <summary>
    /// Utility that creates an <c>${{ eq(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    protected static InlineCondition Equal(InlineExpression expression1, InlineExpression expression2) => new InlineEqualityCondition(true, expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ ne(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    protected static InlineCondition NotEqual(InlineExpression expression1, InlineExpression expression2) => new InlineEqualityCondition(false, expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ gt(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>A <c>gt</c> condition with the specified expressions.</returns>
    protected static InlineCondition Greater(InlineExpression expression1, InlineExpression expression2) => new InlineGreaterCondition(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ lt(expression1, expression2) }}</c> section.
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>A <c>lt</c> condition with the specified expressions.</returns>
    protected static InlineCondition Less(InlineExpression expression1, InlineExpression expression2) => new InlineLessCondition(expression1, expression2);

    /// <summary>
    /// Utility that creates an <c>${{ eq(variables['Build.SourceBranch'], branchName) }}</c> section.
    /// </summary>
    /// <param name="branchName">Branch name to compare with</param>
    /// <returns>An <c>eq</c> condition comparing the current branch with the specified branch name.</returns>
    protected static InlineCondition IsBranch(InlineExpression branchName) => new InlineBranchCondition(branchName, true);

    /// <summary>
    /// Utility that creates an <c>${{ ne(variables['Build.SourceBranch'], branchName) }}</c> section.
    /// </summary>
    /// <param name="branchName">Branch name to compare with</param>
    /// <returns>An <c>ne</c> condition comparing the current branch with the specified branch name.</returns>
    protected static InlineCondition IsNotBranch(InlineExpression branchName) => new InlineBranchCondition(branchName, false);

    /// <summary>
    /// Utility that creates an <c>${{ eq(variables['Build.Reason'], 'PullRequest') }}</c> section.
    /// </summary>
    /// <returns>An <c>eq</c> condition checking if the current build reason is a pull request.</returns>
    protected static InlineCondition IsPullRequest => new InlineBuildReasonCondition("PullRequest", true);

    /// <summary>
    /// Utility that creates an <c>${{ ne(variables['Build.Reason'], 'PullRequest') }}</c> section.
    /// </summary>
    /// <returns>An <c>ne</c> condition checking if the current build reason is not a pull request.</returns>
    protected static InlineCondition IsNotPullRequest => new InlineBuildReasonCondition("PullRequest", false);

    /// <summary>
    /// Starts an <c>${{ each var in collection }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// Each("env", "parameters.environments")
    ///     .StageTemplate("../stages/provision.yml", new()
    ///     {
    ///         { "environment", "${{ env }}" }
    ///     }),
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ each env in parameters.environments }}:
    ///   - template: ../stages/provision.yml
    ///     parameters:
    ///       environment: ${{ env }}
    /// </code>
    /// </summary>
    /// <param name="iterator">Name of the iterator variable</param>
    /// <param name="collection">Collection to iterate over</param>
    /// <returns>An <c>each</c> block with the specified iterator and collection.</returns>
    protected static EachBlock Each(string iterator, string collection) => new(iterator, collection);

    #endregion

    #region Helpers

    internal static readonly Regex NameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    /// <summary>
    /// AzDO allows an empty dependsOn which then forces the stage/job to kick off in parallel.
    /// If dependsOn is omitted, stages/jobs run in the order they are defined.
    /// </summary>
    protected static DependsOn NoDependsOn => [];

    #endregion
}
