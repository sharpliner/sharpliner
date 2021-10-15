using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Task represents the `dotnet restore` command.
    /// </summary>
    public record DotNetRestoreCoreCliTask : DotNetCoreCliTask
    {
        /// <summary>
        /// Specifies the folder in which packages are installed. If no folder is specified, packages are restored into the default NuGet package cache
        ///
        /// Argument aliases: packagesDirectory
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public string? RestoreDirectory
        {
            get => GetString("restoreDirectory");
            init => SetProperty("restoreDirectory", value);
        }

        /// <summary>
        /// Write the additional arguments to be passed to the restore command.
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public string? RestoreArguments
        {
            get => GetString("restoreArguments");
            init => SetProperty("restoreArguments", value);
        }

        /// <summary>
        /// Specifies the amount of detail displayed in the output for the restore command.
        /// quiet, minimal, normal, detailed, diagnostic
        /// </summary>
        [YamlIgnore]
        public BuildVerbosity VerbosityRestore
        {
            get => GetString("verbosityRestore") switch
            {
                "quiet" => BuildVerbosity.Quiet,
                "minimal" => BuildVerbosity.Minimal,
                "normal" => BuildVerbosity.Normal,
                "detailed" => BuildVerbosity.Detailed,
                "diagnostic" => BuildVerbosity.Diagnostic,
                _ => throw new System.NotImplementedException(),
            };
            init => SetProperty("verbosityRestore", value switch
            {
                BuildVerbosity.Quiet => "quiet",
                BuildVerbosity.Minimal => "minimal",
                BuildVerbosity.Normal => "normal",
                BuildVerbosity.Detailed => "detailed",
                BuildVerbosity.Diagnostic => "diagnostic",
                _ => throw new System.NotImplementedException(),
            });
        }

        /// <summary>
        /// Prevents NuGet from using packages from local machine caches
        /// </summary>
        [YamlIgnore]
        public bool NoCache
        {
            get => GetBool("noCache", false);
            init => SetProperty("noCache", value ? "true" : "false");
        }

        /// <summary>
        /// Restore from a given feed
        /// </summary>
        /// <param name="feed">
        /// projectName/feedName for project-scoped feed. FeedName only for organization-scoped feed.
        /// 
        /// Include the selected feed in the generated NuGet.config.
        /// You must have Package Management installed and licensed to select a feed here.
        /// projectName/feedName for project-scoped feed.
        /// FeedName only for organization-scoped feed. Note that this is not supported for the test command.
        /// </param>
        /// <param name="includeNuGetOrg">Include NuGet.org in the generated NuGet.config</param>
        public DotNetRestoreCoreCliTask FromFeed(string feed, bool includeNuGetOrg)
        {
            SetProperty("feedsToUse", "select");
            SetProperty("feedRestore", feed);
            SetProperty("includeNuGetOrg", includeNuGetOrg ? "true" : "value");
            return this;
        }

        /// <summary>
        /// Restore using a NuGet.config file
        /// </summary>
        /// <param name="nugetConfigPath">The NuGet.config in your repository that specifies the feeds from which to restore packages.</param>
        /// <param name="includeNuGetOrg">Include NuGet.org in the generated NuGet.config</param>
        public DotNetRestoreCoreCliTask FromNuGetConfig(string nugetConfigPath)
        {
            SetProperty("feedsToUse", "config");
            SetProperty("nugetConfigPath", nugetConfigPath);
            return this;
        }

        /// <summary>
        /// Include NuGet.org in the generated NuGet.config000 0. 
        /// </summary>
        [YamlIgnore]
        public bool IncludeNuGetOrg
        {
            get => GetBool("includeNuGetOrg", false);
            init => SetProperty("includeNuGetOrg", value ? "true" : "false");
        }

        /// <summary>
        /// The NuGet.config in your repository that specifies the feeds from which to restore packages.
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public string? NuGetConfigPath
        {
            get => GetString("nugetConfigPath");
            init => SetProperty("nugetConfigPath", value);
        }

        /// <summary>
        /// Credentials to use for external registries located in the selected NuGet.config.
        /// For feeds in this organization/collection, leave this blank; the build’s credentials are used automatically
        ///
        /// Argument aliases: externalEndpoints
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public string? ExternalFeedCredentials
        {
            get => GetString("externalFeedCredentials");
            init => SetProperty("externalFeedCredentials", value);
        }
    }

    public enum BuildVerbosity
    {
        Quiet,
        Minimal,
        Normal,
        Detailed,
        Diagnostic,
    }
}
