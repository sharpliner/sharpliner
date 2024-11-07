namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a dotnet task using the <c>DotNetCoreCLI</c> task and <c>UseDotNet</c>.
/// </summary>
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
    /// <para>
    /// Creates the build command version of the DotNetCoreCLI task.
    /// </para>
    /// For example
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.Build("project.csproj", true, "-c Release") with
    ///     {
    ///         WorkingDirectory = "/tmp"
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: build
    ///     projects: project.csproj
    ///     arguments: -c Release
    ///     includeNuGetOrg: true
    ///     workingDirectory: /tmp
    /// </code>
    /// </summary>
    /// <param name="projects">Projects to build</param>
    /// <param name="includeNuGetOrg">Include nuget.org in package sources?</param>
    /// <param name="arguments">Additional arguments</param>
    /// <returns>A new instance of the <see cref="DotNetBuildCoreCliTask"/> with the specified arguments</returns>
    public DotNetBuildCoreCliTask Build(string projects, bool? includeNuGetOrg = null, string? arguments = null) => new()
    {
        Projects = projects,
        Arguments = arguments,
        IncludeNuGetOrg = includeNuGetOrg,
    };

    /// <summary>
    /// <para>
    /// Creates the pack command version of the DotNetCoreCLI task.
    /// </para>
    /// For example
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.Pack("src/*.csproj", "-c Release") with
    ///     {
    ///         NoBuild = true,
    ///         ConfigurationToPack = "Release",
    ///         IncludeSource = true,
    ///         IncludeSymbols = true,
    ///         OutputDir = "/tmp/staging/",
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: pack
    ///     packagesToPack: src/*.csproj
    ///     arguments: -c Release
    ///     nobuild: true
    ///     configurationToPack: Release
    ///     includesource: true
    ///     includesymbols: true
    ///     outputDir: /tmp/staging/
    /// </code>
    /// </summary>
    /// <param name="packagesToPack">
    /// <para>
    /// Pattern to search for csproj or nuspec files to pack. You can separate multiple patterns with a semicolon,
    /// </para>
    /// <para>
    /// and you can make a pattern negative by prefixing it with !.
    /// </para>
    /// Example: <c>**/*.csproj;!**/*.Tests.csproj</c>
    /// Argument aliases: <c>searchPatternPack</c>
    /// </param>
    /// <param name="arguments">Additional arguments</param>
    /// <returns>A new instance of <see cref="DotNetPackCoreCliTask"/> with the specified values.</returns>
    public DotNetPackCoreCliTask Pack(string? packagesToPack = "**/*.csproj", string? arguments = null) => new()
    {
        PackagesToPack = packagesToPack,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>publish</c> command version of the DotNetCoreCLI task.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.Publish("src/*.csproj", true, "-c Release") with
    ///     {
    ///         ModifyOutputPath = true,
    ///         ZipAfterPublish = true,
    ///         Timeout = TimeSpan.FromMinutes(30),
    ///     };
    /// }
    /// </code>
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: publish
    ///     projects: src/*.csproj
    ///     arguments: -c Release
    ///     publishWebProjects: true
    ///     modifyOutputPath: true
    ///     zipAfterPublish: true
    ///   timeoutInMinutes: 30
    /// </code>
    /// </summary>
    /// <param name="projects">Projects to build</param>
    /// <param name="publishWebProjects">
    /// <para>
    /// If true, the projects property value will be skipped and the task will try to find the web projects in the repository and run the publish command on them.
    /// </para>
    /// <para>
    /// Web projects are identified by presence of either a web.config file or wwwroot folder in the directory.
    /// </para>
    /// <para>
    /// In the absence of a web.config file or wwwroot folder, projects that use a web SDK, like <c>Microsoft.NET.Sdk.Web</c>, are selected.
    /// Note that this argument defaults to true if not specified.
    /// </para>
    /// </param>
    /// <param name="arguments">Additional arguments</param>
    /// <returns>A new instance of <see cref="DotNetPublishCoreCliTask"/> with the specified values.</returns>
    public DotNetPublishCoreCliTask Publish(string projects, bool publishWebProjects = true, string? arguments = null) => new()
    {
        Projects = projects,
        Arguments = arguments,
        PublishWebProjects = publishWebProjects,
    };

    /// <summary>
    /// <para>
    /// Creates the push command version of the DotNetCoreCLI task.
    /// </para>
    /// For example
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.Push(arguments: "-c Release") with
    ///     {
    ///         PublishPackageMetadata = true,
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: push
    ///     packagesToPush: $(Build.ArtifactStagingDirectory)/*.nupkg
    ///     arguments: -c Release
    ///     publishPackageMetadata: true
    /// </code>
    /// </summary>
    /// <param name="packagesToPush">The pattern to match or path to nupkg files to be uploaded
    /// Multiple patterns can be separated by a semicolon, and you can make a pattern negative by prefixing it with <c>!</c>.
    /// <para>
    /// Example: <c>**/*.nupkg;!**/*.Tests.nupkg</c>.
    /// </para>
    /// <para>
    /// Argument aliases: <c>searchPatternPush</c>
    /// </para>
    /// </param>
    /// <param name="arguments">Additional arguments</param>
    /// <returns>A new instance of <see cref="DotNetPushCoreCliTask"/> with the specified values.</returns>
    public DotNetPushCoreCliTask Push(string packagesToPush = "$(Build.ArtifactStagingDirectory)/*.nupkg", string? arguments = null) => new()
    {
        PackagesToPush = packagesToPush,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates the test command version of the DotNetCoreCLI task.
    /// </para>
    /// For example
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.Test("*.sln", "/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura") with
    ///     {
    ///         TestRunTitle = "main-test-results"
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: test
    ///     projects: '*.sln'
    ///     arguments: /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
    ///     testRunTitle: main-test-results
    /// </code>
    /// </summary>
    /// <param name="projects">Projects to test</param>
    /// <param name="arguments">Additional arguments</param>
    /// <returns>A new instance of <see cref="DotNetTestCoreCliTask"/> with the specified values.</returns>
    public DotNetTestCoreCliTask Test(string projects, string? arguments = null) => new()
    {
        Projects = projects,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates a custom command version of the DotNetCoreCLI task.
    /// </para>
    /// For example
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Dotnet.CustomCommand("--list-sdks") with
    ///     {
    ///         ContinueOnError = true,
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: DotNetCoreCLI@2
    ///   inputs:
    ///     command: custom
    ///     custom: --list-sdks
    ///   continueOnError: true
    /// </code>
    /// </summary>
    /// <param name="command">.NET command to call</param>
    /// <param name="arguments">Additional arguments for the call</param>
    /// <param name="inputs">Additional arguments defined by the DotNetCoreCLI task</param>
    /// <returns>A new instance of <see cref="Step"/> and not something more specific so that user cannot override Inputs</returns>
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

    /// <summary>
    /// Builder for creating a <c>UseDotNet</c> task.
    /// </summary>
    public class DotNetInstallBuilder
    {
        internal DotNetInstallBuilder()
        {
        }

        /// <summary>
        /// <para>
        /// Creates the <c>dotnet install</c> task for full .NET SDK
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     DotNet.Install.Sdk("8.0.x")
        /// }
        /// </code>
        /// Will generate:
        /// <code lang="yaml">
        /// steps:
        /// - task: UseDotNet@2
        ///   inputs:
        ///     packageType: sdk
        ///     version: 8.0.x
        /// </code>
        /// </summary>
        /// <param name="version">
        /// Specify version of .NET Core SDK or runtime to install.
        /// Versions can be given in the following formats
        /// <code>
        /// 2.x => Install latest in major version.
        /// 3.1.x => Install latest in major and minor version
        /// 3.1.402 => Install exact version
        /// </code>
        /// Find the value of version for installing SDK, from the <c>releases.json</c> for example <see href="https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/3.1/releases.json">releases.json for 3.1</see>
        /// </param>
        /// <param name="includePreviewVersions">
        /// <para>
        /// Select if you want preview versions to be included while searching for latest versions, for example <c>3.1.x</c>.
        /// </para>
        /// <para>
        /// This setting is ignored if you specify an exact version, such as: 3.0.100-preview3-010431
        /// </para>
        /// </param>
        public UseDotNetTask Sdk(string version, bool includePreviewVersions = false) => new(DotNetPackageType.Sdk, version, includePreviewVersions);

        /// <summary>
        /// <para>
        /// Creates the <c>dotnet install</c> task for .NET runtime only
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     DotNet.Install.Runtime("8.0.x")
        /// }
        /// </code>
        /// Will generate:
        /// <code lang="yaml">
        /// steps:
        /// - task: UseDotNet@2
        ///   inputs:
        ///     packageType: runtime
        ///     version: 8.0.x
        /// </code>
        /// </summary>
        /// <param name="version">
        /// Specify version of .NET Core SDK or runtime to install.
        /// Versions can be given in the following formats
        /// <code>
        /// 2.x => Install latest in major version.
        /// 3.1.x => Install latest in major and minor version
        /// 3.1.402 => Install exact version
        /// </code>
        /// Find the value of version for installing Runtime, from the <c>releases.json</c> for example <see href="https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/3.1/releases.json">releases.json for 3.1</see>
        /// </param>
        /// <param name="includePreviewVersions">
        /// <para>
        /// Select if you want preview versions to be included while searching for latest versions, for example <c>3.1.x</c>.
        /// </para>
        /// <para>
        /// This setting is ignored if you specify an exact version, such as: 3.0.100-preview3-010431
        /// </para>
        /// </param>
        public UseDotNetTask Runtime(string version, bool includePreviewVersions = false) => new(DotNetPackageType.Runtime, version, includePreviewVersions);

        /// <summary>
        /// <para>
        /// Select this option to install all SDKs from global.json files.
        /// These files are searched from <c>$(System.DefaultWorkingDirectory)</c>
        /// </para>
        /// <para>
        /// You can change the search root path by setting working directory input.
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     Dotnet.Install.FromGlobalJson("/foo/global.json") with
        ///     {
        ///         WorkingDirectory = "/tmp",
        ///         InstallationPath = "/.dotnet",
        ///     }
        /// }
        /// </code>
        /// Will generate:
        /// <code lang="yaml">
        /// steps:
        /// - task: UseDotNet@2
        ///   inputs:
        ///     useGlobalJson: true
        ///     workingDirectory: /tmp
        ///     installationPath: /.dotnet
        /// </code>
        /// </summary>
        /// <param name="workingDirectory">
        /// Current working directory where the script is run.
        /// Empty is the root of the repo (build) or artifacts (release), which is <c>$(System.DefaultWorkingDirectory)</c>
        /// </param>
        public UseDotNetTask FromGlobalJson(string? workingDirectory = null) => new()
        {
            UseGlobalJson = true,
            WorkingDirectory = workingDirectory,
        };
    }

    /// <summary>
    /// Builder for creating a <c>dotnet restore</c> command using <c>DotNetCoreCLI</c>.
    /// </summary>
    public class DotNetRestoreBuilder
    {
        internal DotNetRestoreBuilder()
        {
        }

        /// <summary>
        /// <para>
        /// Restore target .csproj's
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     Dotnet.Restore.Projects("src/*.csproj") with
        ///     {
        ///         NoCache = true,
        ///         IncludeNuGetOrg = true,
        ///     }
        /// }
        /// </code>
        /// <code lang="yaml">
        /// steps:
        /// - task: DotNetCoreCLI@2
        ///   inputs:
        ///     command: restore
        ///     projects: src/*.csproj
        ///     noCache: true
        ///     includeNuGetOrg: true
        /// </code>
        /// </summary>
        /// <param name="projects">
        /// The path to the csproj file(s) to use You can use wildcards (e.g. <c>**/*.csproj</c> for all .csproj files in all subfolders)
        /// </param>
        public DotNetRestoreCoreCliTask Projects(string projects) => new()
        {
            Projects = projects,
        };

        /// <summary>
        /// <para>
        /// Restore from a given feed
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     Dotnet.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
        ///     {
        ///         ExternalFeedCredentials = "feeds/dotnet-7",
        ///         NoCache = true,
        ///         RestoreDirectory = ".packages",
        ///     }
        /// }
        /// </code>
        /// Will generate:
        /// <code lang="yaml">
        /// steps:
        /// - task: DotNetCoreCLI@2
        ///   inputs:
        ///     command: restore
        ///     includeNuGetOrg: false
        ///     feedsToUse: select
        ///     feedRestore: dotnet-7-preview-feed
        ///     externalFeedCredentials: feeds/dotnet-7
        ///     noCache: true
        ///     restoreDirectory: .packages
        /// </code>
        /// </summary>
        /// <param name="feed">
        /// <c>projectName/feedName</c> for project-scoped feed.
        /// <para>
        /// FeedName only for organization-scoped feed.
        /// </para>
        /// <para>
        /// Include the selected feed in the generated NuGet.config.
        /// </para>
        /// <para>
        /// You must have Package Management installed and licensed to select a feed here.
        /// </para>
        /// </param>
        /// <param name="includeNuGetOrg">Include NuGet.org in the generated NuGet.config</param>
        public DotNetRestoreCoreCliTask FromFeed(string feed, bool? includeNuGetOrg = null) => new()
        {
            IncludeNuGetOrg = includeNuGetOrg,
            Inputs =
                {
                    { DotNetRestoreCoreCliTask.FeedsToUseProperty, DotNetRestoreCoreCliTask.FeedsRestoreValue },
                    { DotNetRestoreCoreCliTask.FeedRestoreProperty, feed },
                }
        };

        /// <summary>
        /// <para>
        /// Restore using a <see href="https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file">NuGet.config</see> file.
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// Steps =
        /// {
        ///     Dotnet.Restore.FromNuGetConfig("src/NuGet.config")
        /// }
        /// </code>
        /// Will generate:
        /// <code lang="yaml">
        /// steps:
        /// - task: DotNetCoreCLI@2
        ///   inputs:
        ///     command: restore
        ///     feedsToUse: config
        ///     nugetConfigPath: src/NuGet.config
        /// </code>
        /// </summary>
        /// <param name="nugetConfigPath">The NuGet.config in your repository that specifies the feeds from which to restore packages.</param>
        public DotNetRestoreCoreCliTask FromNuGetConfig(string nugetConfigPath) => new()
        {
            Inputs =
                {
                    { DotNetRestoreCoreCliTask.FeedsToUseProperty, DotNetRestoreCoreCliTask.ConfigRestoreValue },
                    { DotNetRestoreCoreCliTask.NugetConfigPathProperty, nugetConfigPath },
                }
        };
    }
}
