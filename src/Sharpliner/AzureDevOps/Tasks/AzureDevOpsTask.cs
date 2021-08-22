using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents generic definition of any arbitrary AzDO task.
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=example%2Cparameter-schema#task
    /// </summary>
    public record AzureDevOpsTask : Step
    {
        /// <summary>
        /// Task name in the form 'PublishTestResults@2'.
        /// </summary>
        [YamlMember(Order = 1)]
        public string Task { get; }

        [YamlMember(Order = 101)]
        public TaskInputs Inputs { get; init; } = new();

        public AzureDevOpsTask(string task) : base()
        {
            if (string.IsNullOrEmpty(task))
            {
                throw new System.ArgumentException($"'{nameof(task)}' cannot be null or empty.", nameof(task));
            }

            Task = task;
        }
    }

    public class TaskInputs : Dictionary<string, object> { }
}
