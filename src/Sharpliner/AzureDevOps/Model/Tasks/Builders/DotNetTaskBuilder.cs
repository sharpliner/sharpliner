namespace Sharpliner.AzureDevOps.Tasks
{
    public class DotNetTaskBuilder
    {
        internal DotNetTaskBuilder()
        {
        }

        /// <summary>
        /// Creates the Install command version of the DotNetCoreCLI task.
        /// </summary>
        public DotNetInstallBuilder Install => new();

        /// <summary>
        /// Creates the restore command version of the DotNetCoreCLI task.
        /// </summary>
        public DotNetRestoreBuilder Restore => new();

        /// <summary>
        /// Creates the build command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="projects">Projects to build</param>
        /// <param name="includeNuGetOrg">Include nuget.org in package sources?</param>
        /// <param name="arguments">Additional arguments</param>
        public DotNetBuildCoreCliTask Build(string projects, bool? includeNuGetOrg = null, string? arguments = null) => new()
        {
            Projects = projects,
            Arguments = arguments,
            IncludeNuGetOrg = includeNuGetOrg,
        };

        /// <summary>
        /// Creates the pack command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="packagesToPack">
        /// Pattern to search for csproj or nuspec files to pack. You can separate multiple patterns with a semicolon,
        /// and you can make a pattern negative by prefixing it with !.
        /// Example: **/*.csproj;!**/*.Tests.csproj
        ///
        /// Argument aliases: searchPatternPack
        /// </param>
        /// <param name="arguments">Additional arguments</param>
        public DotNetPackCoreCliTask Pack(string? packagesToPack = "**/*.csproj", string? arguments = null) => new()
        {
            PackagesToPack = packagesToPack,
            Arguments = arguments,
        };

        /// <summary>
        /// Creates the publish command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="projects">Projects to build</param>
        /// <param name="publishWebProjects">
        /// If true, the projects property value will be skipped and the task will try to find the web projects in the repository and
        /// run the publish command on them. Web projects are identified by presence of either a web.config file or wwwroot folder in the directory.
        /// In the absence of a web.config file or wwwroot folder, projects that use a web SDK, like Microsoft.NET.Sdk.Web, are selected.
        ///
        /// Note that this argument defaults to true if not specified.
        /// </param>
        /// <param name="arguments">Additional arguments</param>
        public DotNetPublishCoreCliTask Publish(string projects, bool publishWebProjects = true, string? arguments = null) => new()
        {
            Projects = projects,
            Arguments = arguments,
            PublishWebProjects = publishWebProjects,
        };

        /// <summary>
        /// Creates the push command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="packagesToPush">The pattern to match or path to nupkg files to be uploaded
        /// 
        /// Multiple patterns can be separated by a semicolon, and you can make a pattern negative by prefixing it with !.
        /// Example: **/*.nupkg;!**/*.Tests.nupkg.
        ///
        /// Argument aliases: searchPatternPush
        /// </param>
        /// <param name="arguments">Additional arguments</param>
        public DotNetPushCoreCliTask Push(string packagesToPush = "$(Build.ArtifactStagingDirectory)/*.nupkg", string? arguments = null) => new()
        {
            PackagesToPush = packagesToPush,
            Arguments = arguments,
        };

        /// <summary>
        /// Creates the test command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="projects">Projects to test</param>
        /// <param name="arguments">Additional arguments</param>
        public DotNetTestCoreCliTask Test(string projects, string? arguments = null) => new()
        {
            Projects = projects,
            Arguments = arguments,
        };

        /// <summary>
        /// Creates a custom command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="command">.NET command to call</param>
        /// <param name="arguments">Additional arguments for the call</param>
        /// <param name="inputs">Additional arguments defined by the DotNetCoreCLI task</param>
        // We return Step and not something more specific so that user cannot override Inputs
        public Step CustomCommand(string command, string? arguments = null, TaskInputs? inputs = null)
        {
            var orderedInputs = new TaskInputs()
            {
                { "custom", command },
            };

            if (inputs != null)
            {
                foreach (var pair in inputs)
                {
                    orderedInputs[pair.Key] = pair.Value;
                }
            }

            return new DotNetCoreCliTask("custom")
            {
                Inputs = orderedInputs,
                Arguments = arguments,
            };
        }

        public class DotNetInstallBuilder
        {
            internal DotNetInstallBuilder()
            {
            }

            /// <summary>
            /// Creates the `dotnet install` task for full .NET SDK.
            /// </summary>
            public UseDotNetTask Sdk(string version) => new(DotNetPackageType.Sdk, version);

            /// <summary>
            /// Creates the `dotnet install` task for .NET runtime only.
            /// </summary>
            public UseDotNetTask Runtime(string version) => new(DotNetPackageType.Runtime, version);
        }

        public class DotNetRestoreBuilder
        {
            internal DotNetRestoreBuilder()
            {
            }

            /// <summary>
            /// Restore target .csproj's
            /// </summary>
            /// <param name="projects">
            /// The path to the csproj file(s) to use
            /// You can use wildcards (e.g. **/*.csproj for all .csproj files in all subfolders)
            /// </param>
            public DotNetRestoreCoreCliTask Projects(string projects) => new()
            {
                Inputs =
                {
                    { "projects", projects },
                }
            };

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
            public DotNetRestoreCoreCliTask FromFeed(string feed, bool includeNuGetOrg) => new()
            {
                IncludeNuGetOrg = includeNuGetOrg,
                Inputs =
                {
                    { "feedsToUse", "select" },
                    { "feedRestore", feed },
                }
            };

            /// <summary>
            /// Restore using a NuGet.config file
            /// </summary>
            /// <param name="nugetConfigPath">The NuGet.config in your repository that specifies the feeds from which to restore packages.</param>
            public DotNetRestoreCoreCliTask FromNuGetConfig(string nugetConfigPath) => new()
            {
                Inputs =
                {
                    { "feedsToUse", "config" },
                    { "nugetConfigPath", nugetConfigPath },
                }
            };
        }
    }

    public enum DotNetCommand
    {
        Build,
        Push,
        Pack,
        Publish,
        Restore,
        Run,
        Test,
    }
}
