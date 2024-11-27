using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for restoring NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public record NuGetRestoreCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetRestoreCommandTask"/> class.
        /// </summary>
        public NuGetRestoreCommandTask() : base("restore")
        {
        }

        /// <summary>
        /// Gets or sets the feed to restore packages from.
        /// </summary>
        [YamlIgnore]
        public string? Feed
        {
            get => GetString("feed");
            init => SetProperty("feed", value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include NuGet.org in the generated NuGet.config.
        /// </summary>
        [YamlIgnore]
        public bool? IncludeNuGetOrg
        {
            get => GetBool("includeNuGetOrg", false);
            init => SetProperty("includeNuGetOrg", value);
        }

        /// <summary>
        /// Gets or sets the path to the NuGet.config file.
        /// </summary>
        [YamlIgnore]
        public string? NuGetConfigPath
        {
            get => GetString("nuGetConfigPath");
            init => SetProperty("nuGetConfigPath", value);
        }
    }
}
