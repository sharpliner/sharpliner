using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for pushing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    /// <example>
    /// <code>
    /// var pushTask = new NuGetPushCommandTask
    /// {
    ///     TargetFeed = "https://example.com/nuget/v3/index.json",
    ///     TargetFeedCredentials = "$(System.AccessToken)"
    /// };
    /// </code>
    ///
    /// The corresponding YAML will be:
    ///
    /// <code>
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: push
    ///     targetFeed: 'https://example.com/nuget/v3/index.json'
    ///     targetFeedCredentials: '$(System.AccessToken)'
    /// </code>
    /// </example>
    public abstract record NuGetPushCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPushCommandTask"/> class.
        /// </summary>
        public NuGetPushCommandTask(string nuGetFeedType) : base("push")
        {
            NuGetFeedType = nuGetFeedType;
        }

        /// <summary>
        /// Specifies the pattern to match or path to <c>nupkg</c> files to be uploaded. Multiple patterns can be separated by a semicolon.
        /// <para>
        /// Default value: <c>$(Build.ArtifactStagingDirectory)/**/*.nupkg;!$(Build.ArtifactStagingDirectory)/**/*.symbols.nupkg.</c>
        /// </para>
        /// </summary>
        [YamlIgnore]
        public string[] PackagesToPush
        {
            get => GetString("packagesToPush")!.Split(';');
            init => SetProperty("packagesToPush", string.Join(";", value));
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

        [YamlIgnore]
        internal string NuGetFeedType
        {
            get => GetString("nuGetFeedType")!;
            init => SetProperty("nuGetFeedType", value);
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for pushing NuGet packages to an internal feed in Azure DevOps pipelines.
    /// </summary>
    public record NuGetPushInternalCommandTask : NuGetPushCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPushInternalCommandTask"/> class.
        /// </summary>
        /// <param name="publishVstsFeed">Specifies a feed hosted in this account. You must have Azure Artifacts installed and licensed to select a feed here.</param>
        public NuGetPushInternalCommandTask(string publishVstsFeed) : base("internal")
        {
            PublishVstsFeed = publishVstsFeed;
        }

        /// <summary>
        /// Specifies a feed hosted in this account. You must have Azure Artifacts installed and licensed to select a feed here.
        /// </summary>
        [YamlIgnore]
        public string PublishVstsFeed
        {
            get => GetString("publishVstsFeed")!;
            init => SetProperty("publishVstsFeed", value);
        }

        /// <summary>
        /// Changes the version number of the subset of changed packages within a set of continually published packages.
        /// </summary>
        [YamlIgnore]
        public bool? PublishPackageMetadata
        {
            get => GetBool("publishPackageMetadata", false);
            init => SetProperty("publishPackageMetadata", value);
        }

        /// <summary>
        /// <para>
        /// Reports task success even if some of your packages are rejected with 409 Conflict errors.
        /// </para>
        /// This option is currently only available on Azure Pipelines and Windows agents.
        /// If <c>NuGet.exe</c> encounters a conflict, the task will fail. This option will not work and publishing will fail if you are within a proxy environment.
        /// </summary>
        [YamlIgnore]
        public bool? AllowPackageConflicts
        {
            get => GetBool("allowPackageConflicts", false);
            init => SetProperty("allowPackageConflicts", value);
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for pushing NuGet packages to an external NuGet server.
    /// </summary>
    public record NuGetPushExternalCommandTask : NuGetPushCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPushExternalCommandTask"/> class.
        /// </summary>
        /// <param name="publishFeedCredentials">Specifies the NuGet service connection that contains the external NuGet server’s credentials.</param>
        public NuGetPushExternalCommandTask(string publishFeedCredentials) : base("external")
        {
            PublishFeedCredentials = publishFeedCredentials;
        }

        /// <summary>
        /// Specifies the NuGet service connection that contains the external NuGet server’s credentials.
        /// </summary>
        [YamlIgnore]
        public string PublishFeedCredentials
        {
            get => GetString("publishFeedCredentials")!;
            init => SetProperty("publishFeedCredentials", value);
        }
    }
}
