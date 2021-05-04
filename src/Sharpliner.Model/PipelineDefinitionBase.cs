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
        /// Serializes the pipeline into a YAML string.
        /// </summary>
        public abstract string Publish();

        /// <summary>
        /// Allows the variables[""] notation for conditional definitions.
        /// </summary>
        protected readonly PipelineVariable variables = new();

        protected static Condition<T> And<T>(Condition condition1, Condition condition2) => new AndCondition<T>(condition1, condition2);

        protected static Condition Or<T>(Condition condition1, Condition condition2) => new OrCondition<T>(condition1, condition2);

        protected static Condition<T> Equal<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, true);

        protected static Condition<T> NotEqual<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, false);

        protected static Condition And(Condition condition1, Condition condition2) => new AndCondition(condition1, condition2);

        protected static Condition Or(Condition condition1, Condition condition2) => new OrCondition(condition1, condition2);

        protected static Condition Equal(string expression1, string expression2) => new EqualityDefinitionCondition(expression1, expression2, true);

        protected static Condition NotEqual(string expression1, string expression2) => new EqualityDefinitionCondition(expression1, expression2, false);
    }
}
