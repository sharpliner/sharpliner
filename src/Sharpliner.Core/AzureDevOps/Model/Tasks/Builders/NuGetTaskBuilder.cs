using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create various NuGet tasks in Azure DevOps pipelines.
/// See the official <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/nuget-command-v2">NuGetCommand@2 task reference</see>.
/// </summary>
public class NuGetTaskBuilder
{
    /// <summary>
    /// Creates a <see cref="NuGetAuthenticateTask"/> that configures NuGet tools to authenticate with Azure Artifacts feeds
    /// in this organization or collection, and optionally with feeds outside this organization through NuGet service connections.
    /// </summary>
    /// <param name="nuGetServiceConnections">
    /// Optional NuGet service connection names for feeds outside this organization or collection. For feeds in this organization
    /// or collection, leave this blank; the build's credentials are used automatically.
    /// </param>
    /// <param name="forceReinstallCredentialProvider">
    /// A value indicating whether to overwrite the task-provided credential provider in the user profile even if it is already installed.
    /// This may upgrade or potentially downgrade the credential provider. Default: <c>false</c>.
    /// </param>
    /// <returns>A <see cref="NuGetAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// NuGet.Authenticate(new[] { "myServiceConnection" }, true)
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code>
    /// - task: NuGetAuthenticate@1
    ///   inputs:
    ///     nuGetServiceConnections: myServiceConnection
    ///     forceReinstallCredentialProvider: true
    /// </code>
    /// </example>
    public NuGetAuthenticateTask Authenticate(string[]? nuGetServiceConnections = null, AdoExpression<bool>? forceReinstallCredentialProvider = null)
    {
        var task = new NuGetAuthenticateTask();

        if (forceReinstallCredentialProvider is not null)
        {
            task = task with { ForceReinstallCredentialProvider = forceReinstallCredentialProvider };
        }

        if (nuGetServiceConnections is not null)
        {
            task = task with { NuGetServiceConnections = nuGetServiceConnections };
        }

        return task;
    }

    /// <summary>
    /// Creates a <see cref="NuGetAuthenticateTask"/> that authenticates an Azure Artifacts feed using workload identity federation.
    /// This Azure DevOps Services-only mode is not compatible with <c>nuGetServiceConnections</c>; the official task input alias
    /// for <paramref name="azureDevOpsServiceConnection"/> is <c>workloadIdentityServiceConnection</c>.
    /// </summary>
    /// <param name="azureDevOpsServiceConnection">The Azure DevOps service connection used for workload identity federation.</param>
    /// <param name="feedUrl">
    /// The Azure Artifacts feed URL in NuGet service index format,
    /// such as <c>https://pkgs.dev.azure.com/{ORG_NAME}/{PROJECT}/_packaging/{FEED_NAME}/nuget/v3/index.json</c>.
    /// </param>
    /// <returns>A <see cref="NuGetAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// NuGet.Authenticate("myAzureDevOpsServiceConnection", "https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/nuget/v3/index.json")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code>
    /// - task: NuGetAuthenticate@1
    ///   inputs:
    ///     azureDevOpsServiceConnection: myAzureDevOpsServiceConnection
    ///     feedUrl: https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/nuget/v3/index.json
    /// </code>
    /// </example>
    public NuGetAuthenticateTask Authenticate(AdoExpression<string> azureDevOpsServiceConnection, AdoExpression<string> feedUrl)
    {
        System.ArgumentNullException.ThrowIfNull(azureDevOpsServiceConnection);
        System.ArgumentNullException.ThrowIfNull(feedUrl);

        return new()
        {
            AzureDevOpsServiceConnection = azureDevOpsServiceConnection,
            FeedUrl = feedUrl
        };
    }

    /// <summary>
    /// <para>
    /// Gets a <see cref="NuGetRestoreBuilder"/> instance to create NuGet restore tasks.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// var restoreTask = NuGet.Restore.FromFeed("myFeed") with
    /// {
    ///     IncludeNuGetOrg = true
    /// };
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: restore
    ///     feedsToUse: select
    ///     vstsFeed: myFeed
    ///     includeNuGetOrg: true
    /// </code>
    /// </summary>
    public NuGetRestoreBuilder Restore => new();

    /// <summary>
    /// <para>
    /// Gets a <see cref="NuGetPushBuilder"/> instance to create NuGet push tasks.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// var pushTask = NuGet.Push.ToInternalFeed("myInternalFeed");
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: push
    ///     publishVstsFeed: myInternalFeed
    /// </code>
    /// </summary>
    public NuGetPushBuilder Push => new();

    /// <summary>
    /// <para>
    /// Gets a <see cref="NuGetPackBuilder"/> instance to create NuGet pack tasks.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// NuGet.Pack.ByEnvVar("VERSION")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     versioningScheme: byEnvVar
    ///     versionEnvVar: VERSION
    /// </code>
    /// </summary>
    public NuGetPackBuilder Pack => new();

    /// <summary>
    /// Gets a <see cref="NuGetCustomCommandTask"/> instance to create custom NuGet tasks.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// var customTask = NuGet.Custom(@"config -Set repositoryPath=c:\packages -configfile c:\my.config");
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: custom
    ///     arguments: config -Set repositoryPath=c:\packages -configfile c:\my.config
    /// </code>
    /// </summary>
    /// <returns>A <see cref="NuGetCustomCommandTask"/> instance.</returns>
    public NuGetCustomCommandTask Custom(string arguments) => new(arguments);
}

/// <summary>
/// Provides methods to create NuGet restore tasks.
/// Restore tasks can either generate a <c>NuGet.config</c> from selected feeds (<c>feedsToUse: select</c>)
/// or use a repository <c>NuGet.config</c> (<c>feedsToUse: config</c>).
/// </summary>
public class NuGetRestoreBuilder
{
    /// <summary>
    /// <para>
    /// Creates a NuGetRestoreCommandTask to restore packages from a feed.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// NuGet.Restore.FromFeed("myFeed") with
    /// {
    ///   RestoreSolution = "path/to/solution.sln"
    /// }
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: restore
    ///     feedsToUse: select
    ///     restoreSolution: path/to/solution.sln
    ///     vstsFeed: myFeed
    /// </code>
    /// </summary>
    /// <param name="vstsFeed">The Azure Artifacts/TFS feed to include in the generated <c>NuGet.config</c>. The official input is <c>feedRestore</c>; <c>vstsFeed</c> is the YAML alias emitted for compatibility.</param>
    /// <returns>A NuGetRestoreCommandTask instance.</returns>
    public NuGetRestoreFeedCommandTask FromFeed(string vstsFeed)
    {
        return new()
        {
            VstsFeed = vstsFeed
        };
    }

    /// <summary>
    /// <para>
    /// Creates a NuGetRestoreCommandTask to restore packages from a NuGet.config file.
    /// </para>
    /// <code lang="csharp">
    /// NuGet.Restore.FromNuGetConfig("path/to/NuGet.config") with
    /// {
    ///   RestoreSolution = "path/to/solution.sln"
    /// }
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: restore
    ///     feedsToUse: config
    ///     restoreSolution: path/to/solution.sln
    ///     nugetConfigPath: path/to/NuGet.config
    /// </code>
    /// </summary>
    /// <param name="nugetConfigPath">The path to the <c>NuGet.config</c> file in the repository that specifies the feeds from which to restore packages.</param>
    /// <returns>A NuGetRestoreCommandTask instance.</returns>
    public NuGetRestoreConfigCommandTask FromNuGetConfig(AdoExpression<string> nugetConfigPath)
    {
        return new()
        {
            NuGetConfigPath = nugetConfigPath
        };
    }
}

/// <summary>
/// Provides methods to create NuGet push tasks.
/// Push tasks can target an Azure Artifacts feed in this organization/collection (<c>nuGetFeedType: internal</c>)
/// or an external NuGet server via a NuGet service connection (<c>nuGetFeedType: external</c>).
/// </summary>
public class NuGetPushBuilder
{
    /// <summary>
    /// <para>
    /// Creates a NuGetPushCommandTask to push packages to an internal feed.
    /// </para>
    /// <example>
    /// For example:
    /// <code lang="csharp">
    /// var pushTask = NuGet.Push.ToInternalFeed("myInternalFeed");
    /// </code>
    /// Generated YAML:
    /// <code>
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: push
    ///     nuGetFeedType: internal
    ///     publishVstsFeed: myInternalFeed
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="publishVstsFeed">The Azure Artifacts feed hosted in this organization/collection. The official input is <c>feedPublish</c>; <c>publishVstsFeed</c> is the YAML alias emitted for compatibility.</param>
    /// <returns>A <see cref="NuGetPushInternalCommandTask"/> instance.</returns>
    public NuGetPushInternalCommandTask ToInternalFeed(AdoExpression<string> publishVstsFeed) => new(publishVstsFeed);

    /// <summary>
    /// Creates a NuGetPushCommandTask to push packages to an external feed.
    /// </summary>
    /// <param name="publishFeedCredentials">The NuGet service connection that contains the external NuGet server's credentials. The official input is <c>externalEndpoint</c>; <c>publishFeedCredentials</c> is the YAML alias emitted for compatibility.</param>
    /// <returns>A NuGetPushCommandTask instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// var pushTask = NuGet.Push.ToExternalFeed("myExternalFeedCredentials");
    /// </code>
    /// Generated YAML:
    /// <code>
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: push
    ///     nuGetFeedType: external
    ///     publishFeedCredentials: myExternalFeedCredentials
    /// </code>
    /// </example>
    public NuGetPushExternalCommandTask ToExternalFeed(AdoExpression<string> publishFeedCredentials) => new(publishFeedCredentials);
}

/// <summary>
/// Provides methods to create NuGet pack tasks.
/// Pack tasks support the official versioning schemes <c>off</c>, <c>byPrereleaseNumber</c>, <c>byEnvVar</c>, and <c>byBuildNumber</c>.
/// </summary>
public class NuGetPackBuilder
{
    /// <summary>
    /// Creates a task to pack NuGet packages without versioning.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// NuGet.Pack.WithoutPackageVersioning with
    /// {
    ///     PackagesToPack = "**/*.csproj",
    ///     PackDestination = "$(Build.ArtifactStagingDirectory)"
    /// }
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     versioningScheme: off
    ///     packagesToPack: '**/*.csproj'
    ///     packDestination: $(Build.ArtifactStagingDirectory)
    /// </code>
    /// </summary>
    public NuGetPackCommandTaskOff WithoutPackageVersioning => new();

    /// <summary>
    /// <para>
    /// Creates a task to pack NuGet packages with the version set by a prerelease number.
    /// </para>
    /// <code lang="csharp">
    /// NuGet.Pack.ByPrereleaseNumber("1", "2", "3")
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     versioningScheme: byPrereleaseNumber
    ///     majorVersion: '1'
    ///     minorVersion: '2'
    ///     patchVersion: '3'
    /// </code>
    /// </summary>
    /// <param name="majorVersion">The <c>X</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    /// <param name="minorVersion">The <c>Y</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    /// <param name="patchVersion">The <c>Z</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    /// <returns>A new instance of <see cref="NuGetPackCommandTaskByPrereleaseNumber"/>.</returns>
    public NuGetPackCommandTaskByPrereleaseNumber ByPrereleaseNumber(string majorVersion, string minorVersion, string patchVersion) => new(majorVersion, minorVersion, patchVersion);

    /// <summary>
    /// <para>
    /// Creates a task to pack NuGet packages with the version set by an environment variable.
    /// </para>
    /// The version will be set to the value of the environment variable that has the name specified by the versionEnvVar parameter, e.g. <c>MyVersion</c> (no $, just the environment variable name). 
    /// Make sure the environment variable is set to a proper SemVer, such as <c>1.2.3</c> or <c>1.2.3-beta1</c>.
    /// <code lang="csharp">
    /// NuGet.Pack.ByEnvVar("MY_PACKAGE_VERSION")
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     versioningScheme: byEnvVar
    ///     versionEnvVar: MY_PACKAGE_VERSION
    /// </code>
    /// </summary>
    /// <param name="versionEnvVar">The name of the environment variable that contains the version.</param>
    /// <returns>A new instance of <see cref="NuGetPackCommandTaskByEnvVar"/>.</returns>
    public NuGetPackCommandTaskByEnvVar ByEnvVar(AdoExpression<string> versionEnvVar) => new(versionEnvVar);

    /// <summary>
    /// <para>
    /// Creates a task to pack NuGet packages with the version set by the pipeline run's build number.
    /// </para>
    /// The version will be set using the pipeline run's build number. 
    /// This is the value specified for the pipeline's name property, which gets saved to the <c>BUILD_BUILDNUMBER</c> environment variable). 
    /// Ensure that the build number being used contains a proper SemVer, such as <c>1.0.$(Rev:r)</c>. 
    /// The task will extract the dotted version, <c>1.2.3.4</c>, from the build number string, and use only that portion. 
    /// The rest of the string will be dropped.
    /// <code lang="csharp">
    /// NuGet.Pack.ByBuildNumber with
    /// {
    ///     PackagesToPack = "**/*.csproj"
    /// }
    /// </code>
    /// Generated YAML:
    /// <code lang="yaml">
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     versioningScheme: byBuildNumber
    ///     packagesToPack: '**/*.csproj'
    /// </code>
    /// </summary>
    public NuGetPackCommandTaskByBuildNumber ByBuildNumber => new();
}
