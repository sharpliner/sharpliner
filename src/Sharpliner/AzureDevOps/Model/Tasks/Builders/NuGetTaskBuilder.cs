namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Provides methods to create various NuGet tasks in Azure DevOps pipelines.
    /// </summary>
    public class NuGetTaskBuilder
    {
        /// <summary>
        /// Creates a <see cref="NuGetAuthenticateTask"/> with the specified NuGet service connections and force reinstall credential provider option.
        /// </summary>
        /// <param name="nuGetServiceConnections">The NuGet service connections.</param>
        /// <param name="forceReinstallCredentialProvider">A value indicating whether to force reinstall the credential provider.</param>
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
        public NuGetAuthenticateTask Authenticate(string[]? nuGetServiceConnections = null, bool forceReinstallCredentialProvider = false)
        {
            var task = new NuGetAuthenticateTask
            {
                ForceReinstallCredentialProvider = forceReinstallCredentialProvider
            };

            if (nuGetServiceConnections is not null)
            {
                task.NuGetServiceConnections = nuGetServiceConnections,
            }

            return task;
        }

        /// <summary>
        /// <para>
        /// Gets a <see cref="NuGetRestoreBuilder"/> instance to create NuGet restore tasks.
        /// </para>
        /// For example:
        /// <code lang="csharp">
        /// var restoreTask = NuGet.Restore.FromFeed("myFeed", true);
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code lang="yaml">
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: restore
        ///     feedsToUse: myFeed
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
        public NuGetCustomCommandTask Custom(string? arguments = null) => new()
        {
            Arguments = arguments
        };
    }

    /// <summary>
    /// Provides methods to create NuGet restore tasks.
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
        ///     feedsToUse: myFeed
        /// </code>
        /// </summary>
        /// <param name="vstsFeed">The feed to restore packages from.</param>
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
        /// NuGet.Restore.FromNuGetConfig("path/to/NuGet.config");
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
        /// <param name="nugetConfigPath">The path to the NuGet.config file.</param>
        /// <returns>A NuGetRestoreCommandTask instance.</returns>
        public NuGetRestoreConfigCommandTask FromNuGetConfig(string nugetConfigPath)
        {
            return new()
            {
                NuGetConfigPath = nugetConfigPath
            };
        }
    }

    /// <summary>
    /// Provides methods to create NuGet push tasks.
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
        /// <returns>A <see cref="NuGetPushInternalCommandTask"/> instance.</returns>
        public NuGetPushInternalCommandTask ToInternalFeed(string publishVstsFeed) => new(publishVstsFeed);

        /// <summary>
        /// Creates a NuGetPushCommandTask to push packages to an external feed.
        /// </summary>
        /// <param name="publishFeedCredentials">The publish feed credentials.</param>
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
        ///     externalFeedCredentials: myExternalFeedCredentials
        /// </code>
        /// </example>
        public NuGetPushExternalCommandTask ToExternalFeed(string publishFeedCredentials) => new(publishFeedCredentials);
    }

    /// <summary>
    /// Provides methods to create NuGet pack tasks.
    /// </summary>
    public class NuGetPackBuilder
    {
        /// <summary>
        /// Creates a task to pack NuGet packages without versioning.
        /// </summary>
        public NuGetPackCommandTaskOff WithoutPackageVersioning => new();

        /// <summary>
        /// <para>
        /// Creates a task to pack NuGet packages with the version set by a prerelease number.
        /// </para>
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
        /// </summary>
        /// <param name="versionEnvVar">The name of the environment variable that contains the version.</param>
        /// <returns>A new instance of <see cref="NuGetPackCommandTaskByEnvVar"/>.</returns>
        public NuGetPackCommandTaskByEnvVar ByEnvVar(string versionEnvVar) => new(versionEnvVar);

        /// <summary>
        /// <para>
        /// Creates a task to pack NuGet packages with the version set by the pipeline run's build number.
        /// </para>
        /// The version will be set using the pipeline run's build number. 
        /// This is the value specified for the pipeline's name property, which gets saved to the <c>BUILD_BUILDNUMBER</c> environment variable). 
        /// Ensure that the build number being used contains a proper SemVer, such as <c>1.0.$(Rev:r)</c>. 
        /// The task will extract the dotted version, <c>1.2.3.4</c>, from the build number string, and use only that portion. 
        /// The rest of the string will be dropped.
        /// </summary>
        public NuGetPackCommandTaskByBuildNumber ByBuildNumber => new();
    }
}
