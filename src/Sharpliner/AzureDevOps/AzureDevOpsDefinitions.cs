using Sharpliner.AzureDevOps.Tasks;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// This is a common ancestor for AzDO related definitions (pipelines, templates..) containing useful macros.
    /// </summary>
    public abstract class AzureDevOpsDefinitions : PipelineDefinitionBase
    {
        /// <summary>
        /// Start an ${{ if () }} section.
        /// </summary>
        protected static ConditionBuilder If => new();

        /// <summary>
        /// Reference a YAML template.
        /// </summary>
        /// <param name="path">Relative path to the YAML file with the template</param>
        protected static ConditionedDefinition<VariableBase> Template(string path)
            => new Template<VariableBase>(path);

        /// <summary>
        /// Reference a YAML template.
        /// </summary>
        /// <param name="path">Relative path to the YAML file with the template</param>
        /// <param name="parameters">Values for template parameters</param>
        protected static ConditionedDefinition<T> Template<T>(string path, TemplateParameters? parameters = null)
            => new Template<T>(path, parameters);

        /// <summary>
        /// Allows the variables[""] notation for conditional definitions.
        /// </summary>
        protected readonly PipelineVariable variables = new();

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value));

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value));

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));

        /// <summary>
        /// References a variable group.
        /// </summary>
        /// <param name="name">Group name</param>
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name));

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
        /// Creates a new job.
        /// </summary>
        protected static Job Job(string jobName, string? displayName = null) => new Job(jobName) with { DisplayName = displayName! };

        /// <summary>
        /// Creates an UseDotNet or DotNetCoreCLI task.
        /// </summary>
        protected static DotNetTaskBuilder DotNet { get; } = new();

        protected static Condition<T> And<T>(Condition condition1, Condition condition2) => new AndCondition<T>(condition1, condition2);

        protected static Condition Or<T>(Condition condition1, Condition condition2) => new OrCondition<T>(condition1, condition2);

        protected static Condition<T> Equal<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, true);

        protected static Condition<T> NotEqual<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, false);

        protected static Condition And(Condition condition1, Condition condition2) => new AndCondition(condition1, condition2);

        protected static Condition Or(Condition condition1, Condition condition2) => new OrCondition(condition1, condition2);

        protected static Condition Equal(string expression1, string expression2) => new EqualityCondition(expression1, expression2, true);

        protected static Condition NotEqual(string expression1, string expression2) => new EqualityCondition(expression1, expression2, false);

        protected static Condition IsBranch(string branchName) => new BranchCondition(branchName, true);

        protected static Condition IsNotBranch(string branchName) => new BranchCondition(branchName, false);

        protected static Condition IsPullRequest => new BuildReasonCondition("'PullRequest'", true);

        protected static Condition IsNotPullRequest => new BuildReasonCondition("'PullRequest'", false);
    }
}
