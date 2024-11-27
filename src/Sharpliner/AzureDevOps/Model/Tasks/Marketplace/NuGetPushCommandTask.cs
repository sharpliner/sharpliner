using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for pushing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public record NuGetPushCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPushCommandTask"/> class.
        /// </summary>
        public NuGetPushCommandTask() : base("push")
        {
        }

        /// <summary>
        /// Gets or sets the target feed for the push command.
        /// </summary>
        [YamlIgnore]
        public string? TargetFeed
        {
            get => GetString("targetFeed");
            init => SetProperty("targetFeed", value);
        }

        /// <summary>
        /// Gets or sets the target feed credentials for the push command.
        /// </summary>
        [YamlIgnore]
        public string? TargetFeedCredentials
        {
            get => GetString("targetFeedCredentials");
            init => SetProperty("targetFeedCredentials", value);
        }
    }
}
