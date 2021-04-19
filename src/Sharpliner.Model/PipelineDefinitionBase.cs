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

        protected static Variable Variable(string name, string value) => new(name, value);
        protected static Variable Variable(string name, int value) => new(name, value);
        protected static Variable Variable(string name, bool value) => new(name, value);
        protected static VariableGroup Group(string name) => new(name);
        protected static VariableDefinitionConditionBuilder If => new();

        protected static VariableDefinitionCondition And(VariableDefinitionCondition condition1, VariableDefinitionCondition condition2)
            => new AndVariableDefinitionCondition(condition1, condition2);
        protected static VariableDefinitionCondition Or(VariableDefinitionCondition condition1, VariableDefinitionCondition condition2)
            => new OrVariableDefinitionCondition(condition1, condition2);
        protected static VariableDefinitionCondition Equal(string expression1, string expression2)
            => new EqualityVariableDefinitionCondition(expression1, expression2, true);
        protected static VariableDefinitionCondition NotEqual(string expression1, string expression2)
            => new EqualityVariableDefinitionCondition(expression1, expression2, false);
    }

    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
    }

    public abstract class GitHubActionsPipelineDefinition : PipelineDefinitionBase
    {
    }
}
