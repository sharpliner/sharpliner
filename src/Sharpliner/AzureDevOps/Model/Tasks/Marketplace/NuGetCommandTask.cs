using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the abstract NuGetCommand@2 task in Azure DevOps pipelines.
    /// </summary>
    public abstract record NuGetCommandTask : AzureDevOpsTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetCommandTask"/> class.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        protected NuGetCommandTask(string command) : base("NuGetCommand@2")
        {
            Command = command;
        }

        [YamlIgnore]
        internal string Command
        {
            get => GetString("command")!;
            init => SetProperty("command", value);
        }
    }
}
