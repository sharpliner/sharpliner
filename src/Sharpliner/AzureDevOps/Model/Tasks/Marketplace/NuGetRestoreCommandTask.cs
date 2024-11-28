using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for restoring NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public abstract record NuGetRestoreCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetRestoreCommandTask"/> class.
        /// </summary>
        protected NuGetRestoreCommandTask(string feedsToUse) : base("restore")
        {
            FeedsToUse = feedsToUse;
        }

        /// <summary>
        /// Specifies the path to the solution, <c>packages.config</c>, or <c>project.json</c> file that references the packages to be restored.
        /// </summary>
        [YamlIgnore]
        public string? RestoreSolution
        {
            get => GetString("restoreSolution")!;
            init => SetProperty("restoreSolution", value);
        }

        /// <summary>
        /// Prevents NuGet from using packages from local machine caches when set to <c>true</c>.
        /// </summary>
        [YamlIgnore]
        public bool? NoCache
        {
            get => GetBool("noCache", false);
            init => SetProperty("noCache", value);
        }

        [YamlIgnore]
        internal string FeedsToUse
        {
            get => GetString("feedsToUse")!;
            init => SetProperty("feedsToUse", value);
        }
    }

    public record NuGetRestoreFeedCommandTask : NuGetRestoreCommandTask
    {
        public NuGetRestoreFeedCommandTask() : base("select")
        {
        }

        /// <summary>
        /// Gets or sets the vstsFeed to restore packages from.
        /// </summary>
        [YamlIgnore]
        public string? VstsFeed
        {
            get => GetString("vstsFeed");
            init => SetProperty("vstsFeed", value);
        }

        /// <summary>
        /// Includes NuGet.org in the generated <c>NuGet.config</c>.
        /// </summary>
        [YamlIgnore]
        public bool? IncludeNuGetOrg
        {
            get => GetBool("includeNuGetOrg", false);
            init => SetProperty("includeNuGetOrg", value);
        }
    }

    public record NuGetRestoreConfigCommandTask : NuGetRestoreCommandTask
    {
        public NuGetRestoreConfigCommandTask() : base("config")
        {
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

        /// <summary>
        /// Specifies the credentials to use for external registries located in the selected <c>NuGet.config</c>.
        /// This is the name of your NuGet service connection. 
        /// For feeds in this organization or collection, leave this blank; the build's credentials are used automatically.
        /// </summary>
        [YamlIgnore]
        public string ExternalFeedCredentials
        {
            get => GetString("externalFeedCredentials")!;
            init => SetProperty("externalFeedCredentials", value);
        } 
    }
}
