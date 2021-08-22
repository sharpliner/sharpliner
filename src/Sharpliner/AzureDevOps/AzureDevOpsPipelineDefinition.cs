using Sharpliner.AzureDevOps.Tasks;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinitionBase<TPipeline> : PipelineDefinitionBase where TPipeline : AzureDevOpsPipelineBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract TPipeline Pipeline { get; }

        public override void Validate() => Pipeline.Validate();

        public override string Serialize() => PrettifyYaml(SharplinerSerializer.Serialize(Pipeline));

        #region Syntax sugar for common AzDO pipelines elements

        protected static ConditionBuilder If => new();

        protected static ConditionedDefinition<VariableBase> Template(string path)
            => new Template<VariableBase>(path);
        protected static ConditionedDefinition<T> Template<T>(string path, TemplateParameters? parameters = null)
            => new Template<T>(path, parameters);

        /// <summary>
        /// Allows the variables[""] notation for conditional definitions.
        /// </summary>
        protected readonly PipelineVariable variables = new();
        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name));

        protected static BashTaskBuilder Bash { get; } = new();
        protected static ScriptTaskBuilder Script { get; } = new();
        protected static PowershellTaskBuilder Powershell { get; } = new(false);
        protected static PowershellTaskBuilder Pwsh { get; } = new(true);
        protected static PublishTask Publish(string artifactName, string filePath, string? displayName = null)
            => new PublishTask(filePath) with
            {
                DisplayName = displayName!,
                Artifact = artifactName!,
            };
        protected static CheckoutTaskBuilder Checkout { get; } = new();
        protected static DownloadTaskBuilder Download { get; } = new();
        protected static AzureDevOpsTask Task(string taskName, string? displayName = null) => new AzureDevOpsTask(taskName) with { DisplayName = displayName! };
        protected static Job Job(string jobName, string? displayName = null) => new Job(jobName) with { DisplayName = displayName! };
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

        #endregion
    }

    public abstract class AzureDevOpsPipelineDefinition : AzureDevOpsPipelineDefinitionBase<AzureDevOpsPipeline>
    {
    }

    public abstract class SingleStageAzureDevOpsPipelineDefinition : AzureDevOpsPipelineDefinitionBase<SingleStageAzureDevOpsPipeline>
    {
    }
}
