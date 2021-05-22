using System;
using System.Text.RegularExpressions;
using Sharpliner.AzureDevOps.Tasks;
using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinitionBase<TPipeline> : PipelineDefinitionBase where TPipeline : class
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract TPipeline Pipeline { get; }

        protected static ConditionedDefinition<VariableBase> Template(string path)
            => new Template<VariableBase>(path);
        protected static ConditionedDefinition<T> Template<T>(string path, TemplateParameters? parameters = null)
            => new Template<T>(path, parameters);

        protected static ConditionBuilder<VariableBase> If => new();
        protected static ConditionBuilder<T> If_<T>() => new();

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name));
        protected static BashTaskBuilder Bash { get; } = new();
        protected static PowershellTaskBuilder Powershell { get; } = new(false);
        protected static PowershellTaskBuilder Pwsh { get; } = new(true);
        protected static PublishTask Publish(string displayName, string filePath) => new(displayName, filePath);
        protected static PublishTask Publish(string filePath) => new(null, filePath);
        protected static CheckoutTaskBuilder Checkout { get; } = new();
        protected static DownloadTaskBuilder Download { get; } = new();

        public override string Serialize() => Prettify(SharplinerSerializer.Serialize(Pipeline));

        private static string Prettify(string yaml)
        {
            // Add empty new lines to make text more readable
            yaml = Regex.Replace(yaml, "((\r?\n)[a-zA-Z]+:)", Environment.NewLine + "$1");
            yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", Environment.NewLine + "$1");
            yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?\\${{ ?if[^\n]+\n)", Environment.NewLine + "$1");
            yaml = Regex.Replace(yaml, "(:\r?\n\r?\n)", ":" + Environment.NewLine);

            return yaml;
        }
    }

    public abstract class AzureDevOpsPipelineDefinition : AzureDevOpsPipelineDefinitionBase<AzureDevOpsPipeline>
    {
    }

    public abstract class SingleStageAzureDevOpsPipelineDefinition : AzureDevOpsPipelineDefinitionBase<SingleStageAzureDevOpsPipeline>
    {
    }
}
