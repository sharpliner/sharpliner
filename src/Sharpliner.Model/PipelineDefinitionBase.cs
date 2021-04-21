using Sharpliner.Model.AzureDevOps;

namespace Sharpliner.Model.Definition
{
    /// <summary>
    /// To define a pipeline, inherit from this class and implement the missing fields.
    /// The type will be located via reflection and the pipeline will be compiled into a YAML file.
    /// Define one pipeline child class per resulting file.
    /// </summary>
    public abstract class PipelineDefinitionBase
    {
        /// <summary>
        /// Path to the YAML file where this pipeline will be exported to.
        /// When you build the project, the pipeline will be saved into a file on this path.
        /// Example: "/pipelines/ci.yaml"
        /// </summary>
        public abstract string TargetFile { get; }

        /// <summary>
        /// Override this to define where the resulting YAML should be stored (together with TargetFile).
        /// Default is RelativeToCurrentDir.
        /// </summary>
        public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract Pipeline Pipeline { get; }

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(null, new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(null, new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(null, new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(null, new VariableGroup(name));
        protected static DefinitionConditionBuilder<VariableBase> If => new();

        protected static DefinitionCondition<T> And<T>(DefinitionCondition condition1, DefinitionCondition condition2)
            => new AndDefinitionCondition<T>(condition1, condition2);

        protected static DefinitionCondition And(DefinitionCondition condition1, DefinitionCondition condition2)
            => new AndDefinitionCondition(condition1, condition2);

        protected static DefinitionCondition Or(DefinitionCondition condition1, DefinitionCondition condition2)
            => new OrDefinitionCondition(condition1, condition2);

        protected static DefinitionCondition Or<T>(DefinitionCondition condition1, DefinitionCondition condition2)
            => new OrDefinitionCondition<T>(condition1, condition2);

        protected static DefinitionCondition Equal(string expression1, string expression2)
            => new EqualityDefinitionCondition(expression1, expression2, true);

        protected static DefinitionCondition<T> Equal<T>(string expression1, string expression2)
            => new EqualityDefinitionCondition<T>(expression1, expression2, true);

        protected static DefinitionCondition<T> NotEqual<T>(string expression1, string expression2)
            => new EqualityDefinitionCondition<T>(expression1, expression2, false);

        protected static DefinitionCondition NotEqual(string expression1, string expression2)
            => new EqualityDefinitionCondition(expression1, expression2, false);
    }

    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
    }

    public abstract class GitHubActionsPipelineDefinition : PipelineDefinitionBase
    {
    }
}
