using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/command-line?view=azure-devops&tabs=yaml
    /// </summary>
    public record CommandLineTask : Step
    {
        /// <summary>
        /// Required if Type is inline, contents of the script.
        /// </summary>
        [YamlMember(Alias = "script", Order = 1)]
        public string Contents { get; }

        /// <summary>
        /// Specify the working directory in which you want to run the command.
        /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
        /// </summary>
        [YamlMember(Order = 113)]
        public string? WorkingDirectory { get; init; }

        /// <summary>
        /// If this is true, this task will fail if any errors are written to stderr.
        /// Defaults to 'false'.
        /// </summary>
        [YamlMember(Order = 200)]
        public bool FailOnStdErr { get; init; } = false;

        public CommandLineTask(string displayName, params string[] scriptLines)
            : base(displayName)
        {
            if (scriptLines is null)
            {
                throw new ArgumentNullException(nameof(scriptLines));
            }

            Contents = string.Join("\n", scriptLines);
        }
    }
}
