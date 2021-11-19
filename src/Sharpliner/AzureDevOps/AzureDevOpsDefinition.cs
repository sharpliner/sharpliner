﻿using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This is a common ancestor for AzDO related definitions (pipelines, templates..) containing useful macros.
/// </summary>
public abstract class AzureDevOpsDefinition
{
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
    /// Allows the variables[""] notation for conditional definitions.
    /// </summary>
    protected readonly PipelineVariable variables = new();

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

    public sealed class PipelineVariable
    {
        public string this[string variableName] => $"variables['{variableName}']";
    }
}
