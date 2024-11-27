using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for executing custom NuGet commands in Azure DevOps pipelines.
    /// </summary>
    public record NuGetCustomCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetCustomCommandTask"/> class.
        /// </summary>
        public NuGetCustomCommandTask() : base("custom")
        {
        }

        /// <summary>
        /// Gets or sets the custom command to execute.
        /// </summary>
        [YamlIgnore]
        public string? Command
        {
            get => GetString("command");
            init => SetProperty("command", value);
        }

        /// <summary>
        /// Gets or sets the arguments for the custom command.
        /// </summary>
        [YamlIgnore]
        public string? Arguments
        {
            get => GetString("arguments");
            init => SetProperty("arguments", value);
        }
    }
}
