using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This is a common ancestor for AzDO related definitions (pipelines, templates..) containing useful macros.
/// </summary>
public abstract class AzureDevOpsDefinition
{
    internal static readonly Regex NameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    /// <summary>
    /// Start an ${{ if () }} section.
    /// </summary>
    protected static ConditionBuilder If => new();

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

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> StageLibrary(StageLibrary library)
        => new LibraryReference<Stage>(library);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static Conditioned<JobBase> JobLibrary(JobLibrary library)
        => new LibraryReference<JobBase>(library);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> StepLibrary(StepLibrary library)
        => new LibraryReference<Step>(library);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static Conditioned<VariableBase> VariableLibrary(VariableLibrary library)
        => new LibraryReference<VariableBase>(library);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> ExpandStages(params Conditioned<Stage>[] stages)
        => new LibraryReference<Stage>(stages);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static Conditioned<Job> ExpandJobs(params Conditioned<Job>[] jobs)
        => new LibraryReference<Job>(jobs);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> ExpandSteps(params Conditioned<Step>[] steps)
        => new LibraryReference<Step>(steps);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static Conditioned<VariableBase> ExpandVariables(params Conditioned<VariableBase>[] variables)
        => new LibraryReference<VariableBase>(variables);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> ExpandStages(IEnumerable<Conditioned<Stage>> stages)
        => new LibraryReference<Stage>(stages);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static Conditioned<Job> ExpandJobs(IEnumerable<Conditioned<Job>> jobs)
        => new LibraryReference<Job>(jobs);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> ExpandSteps(IEnumerable<Conditioned<Step>> steps)
        => new LibraryReference<Step>(steps);

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static Conditioned<VariableBase> ExpandVariables(IEnumerable<Conditioned<VariableBase>> variables)
        => new LibraryReference<VariableBase>(variables);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> ExpandStages(params Stage[] stages)
        => ExpandStages(stages.Select(x => new Conditioned<Stage>(x)));

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static Conditioned<Job> ExpandJobs(params Job[] jobs)
        => ExpandJobs(jobs.Select(x => new Conditioned<Job>(x)));

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> ExpandSteps(params Step[] steps)
        => ExpandSteps(steps.Select(x => new Conditioned<Step>(x)));

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static Conditioned<VariableBase> ExpandVariables(params VariableBase[] variables)
        => ExpandVariables(variables.Select(x => new Conditioned<VariableBase>(x)));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> ExpandStages(IEnumerable<Stage> stages)
        => ExpandStages(stages.ToArray());

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    protected static Conditioned<Job> ExpandJobs(IEnumerable<Job> jobs)
        => ExpandJobs(jobs.ToArray());

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> ExpandSteps(IEnumerable<Step> steps)
        => ExpandSteps(steps.ToArray());

    /// <summary>
    /// Reference a step library (series of library variables).
    /// </summary>
    protected static Conditioned<VariableBase> ExpandVariables(IEnumerable<VariableBase> variables)
        => ExpandVariables(variables.ToArray());

    /// <summary>
    /// Reference a stage library (series of library stages).
    /// </summary>
    protected static Conditioned<Stage> StageLibrary<T>() where T : StageLibrary, new()
        => CreateLibraryRef<T, Stage>();

    /// <summary>
    /// Reference a job library (series of library jobs).
    /// </summary>
    protected static Conditioned<JobBase> JobLibrary<T>() where T : JobLibrary, new()
        => CreateLibraryRef<T, JobBase>();

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    protected static Conditioned<Step> StepLibrary<T>() where T : StepLibrary, new()
        => CreateLibraryRef<T, Step>();

    /// <summary>
    /// Reference a variable library (set of variable definition).
    /// </summary>
    protected static Conditioned<VariableBase> VariableLibrary<T>() where T : VariableLibrary, new()
        => CreateLibraryRef<T, VariableBase>();

    /// <summary>
    /// Helper method to create instances of LibraryReference.
    /// </summary>
    /// <typeparam name="TLibrary">User's implementation type of the library</typeparam>
    /// <typeparam name="TDefinition">Definition type (Stage/Job/Step/Variable)</typeparam>
    internal static LibraryReference<TDefinition> CreateLibraryRef<TLibrary, TDefinition>()
        where TLibrary : DefinitionLibrary<TDefinition>, new()
        => new(CreateInstance<TLibrary>());

    /// <summary>
    /// Helper method to create instances of T.
    /// </summary>
    internal static T CreateInstance<T>() where T : new() => (T)Activator.CreateInstance(typeof(T))!;

    /// <summary>
    /// Allows the variables[""] notation for conditional definitions.
    /// </summary>
    protected static readonly VariableReference variables = new();

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static Conditioned<VariableBase> Variable(string name, string value) => new(new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static Conditioned<VariableBase> Variable(string name, int value) => new(new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    protected static Conditioned<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));

    /// <summary>
    /// References a variable group.
    /// </summary>
    /// <param name="name">Group name</param>
    protected static Conditioned<VariableBase> Group(string name) => new(new VariableGroup(name));

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
    protected static PowershellTaskBuilder Powershell { get; } = new(false);

    /// <summary>
    /// Creates a pwsh task.
    /// </summary>
    protected static PowershellTaskBuilder Pwsh { get; } = new(true);

    /// <summary>
    /// Creates a publish task.
    /// </summary>
    protected static PublishTask Publish(string artifactName, string filePath, string? displayName = null)
        => new PublishTask(filePath) with
        {
            DisplayName = displayName!,
            Artifact = artifactName!,
        };

    /// <summary>
    /// Creates a checkout task.
    /// </summary>
    protected static CheckoutTaskBuilder Checkout { get; } = new();

    /// <summary>
    /// Creates a download task.
    /// </summary>
    protected static DownloadTaskBuilder Download { get; } = new();

    /// <summary>
    /// Creates a generic pipeline task.
    /// </summary>
    protected static AzureDevOpsTask Task(string taskName, string? displayName = null) => new AzureDevOpsTask(taskName) with { DisplayName = displayName! };

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
    /// AzDO allows an empty dependsOn which then forces the stage/job to kick off in parallel.
    /// If dependsOn is omitted, stages/jobs run in the order they are defined.
    /// </summary>
    protected static ConditionedList<string> NoDependsOn => new EmptyDependsOn();

    /// <summary>
    /// Use this to specify any custom condition (in case you miss some operator or expression).
    /// </summary>
    protected static Condition Condition(string condition) => new CustomCondition(condition);

    protected static Condition<T> And<T>(params string[] expressions) => new AndCondition<T>(expressions);

    protected static Condition Or<T>(params string[] expressions) => new OrCondition<T>(expressions);

    protected static Condition Xor<T>(string expression1, string expression2) => new XorCondition<T>(expression1, expression2);

    protected static Condition<T> And<T>(params Condition[] expressions) => new AndCondition<T>(expressions);

    protected static Condition Or<T>(params Condition[] expressions) => new OrCondition<T>(expressions);

    protected static Condition Xor<T>(Condition expression1, Condition expression2) => new XorCondition<T>(expression1, expression2);

    protected static Condition<T> Equal<T>(string expression1, string expression2) => new EqualityCondition<T>(true, expression1, expression2);

    protected static Condition<T> NotEqual<T>(string expression1, string expression2) => new EqualityCondition<T>(false, expression1, expression2);

    protected static Condition Contains<T>(string needle, string haystack) => new ContainsCondition<T>(needle, haystack);

    protected static Condition StartsWith<T>(string needle, string haystack) => new StartsWithCondition<T>(needle, haystack);

    protected static Condition EndsWith<T>(string needle, string haystack) => new EndsWithCondition<T>(needle, haystack);

    protected static Condition ContainsValue<T>(string needle, params string[] haystack) => new ContainsValueCondition<T>(needle, haystack);

    protected static Condition In<T>(string needle, params string[] haystack) => new InCondition<T>(needle, haystack);

    protected static Condition NotIn<T>(string needle, params string[] haystack) => new NotInCondition<T>(needle, haystack);

    protected static Condition Greater<T>(string expression1, string expression2) => new GreaterCondition<T>(expression1, expression2);

    protected static Condition Less<T>(string expression1, string expression2) => new LessCondition<T>(expression1, expression2);

    protected static Condition And(params string[] expressions) => new AndCondition(expressions);

    protected static Condition Or(params string[] expressions) => new OrCondition(expressions);

    protected static Condition Xor(string condition1, string condition2) => new XorCondition(condition1, condition2);

    protected static Condition And(params Condition[] expressions) => new AndCondition(expressions);

    protected static Condition Or(params Condition[] expressions) => new OrCondition(expressions);

    protected static Condition Xor(Condition expression1, Condition expression2) => new XorCondition(expression1, expression2);

    protected static Condition Contains(string needle, string haystack) => new ContainsCondition(needle, haystack);

    protected static Condition StartsWith(string needle, string haystack) => new StartsWithCondition(needle, haystack);

    protected static Condition EndsWith(string needle, string haystack) => new EndsWithCondition(needle, haystack);

    protected static Condition In(string needle, params string[] haystack) => new InCondition(needle, haystack);

    protected static Condition NotIn(string needle, params string[] haystack) => new NotInCondition(needle, haystack);

    protected static Condition ContainsValue(string needle, params string[] haystack) => new ContainsValueCondition(needle, haystack);

    protected static Condition Equal(string expression1, string expression2) => new EqualityCondition(true, expression1, expression2);

    protected static Condition NotEqual(string expression1, string expression2) => new EqualityCondition(false, expression1, expression2);

    protected static Condition Greater(string expression1, string expression2) => new GreaterCondition(expression1, expression2);

    protected static Condition Less(string expression1, string expression2) => new LessCondition(expression1, expression2);

    protected static Condition IsBranch(string branchName) => new BranchCondition(branchName, true);

    protected static Condition IsNotBranch(string branchName) => new BranchCondition(branchName, false);

    protected static Condition IsPullRequest => new BuildReasonCondition("'PullRequest'", true);

    protected static Condition IsNotPullRequest => new BuildReasonCondition("'PullRequest'", false);
}
