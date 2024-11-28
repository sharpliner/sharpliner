namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// The NuGetTaskBuilder class provides methods to create various NuGet tasks in Azure DevOps pipelines.
    /// These tasks include NuGetAuthenticateTask, Restore, Push, Pack, and Custom.
    /// </summary>
    /// <example>
    /// <code>
    /// var authenticateTask = NuGet.Authenticate(new[] { "myServiceConnection" }, true);
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code>
    /// - task: NuGetAuthenticate@1
    ///   inputs:
    ///     nuGetServiceConnections: myServiceConnection
    ///     forceReinstallCredentialProvider: true
    /// </code>
    /// </example>
    public class NuGetTaskBuilder
    {
        /// <summary>
        /// Creates a NuGetAuthenticateTask with the specified NuGet service connections and force reinstall credential provider option.
        /// </summary>
        /// <param name="nuGetServiceConnections">The NuGet service connections.</param>
        /// <param name="forceReinstallCredentialProvider">A value indicating whether to force reinstall the credential provider.</param>
        /// <returns>A NuGetAuthenticateTask instance.</returns>
        /// <example>
        /// <code>
        /// var authenticateTask = NuGet.Authenticate(new[] { "myServiceConnection" }, true);
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
            return new NuGetAuthenticateTask
            {
                NuGetServiceConnections = nuGetServiceConnections,
                ForceReinstallCredentialProvider = forceReinstallCredentialProvider
            };
        }

        /// <summary>
        /// Gets a NuGetRestoreBuilder instance to create NuGet restore tasks.
        /// </summary>
        /// <example>
        /// <code>
        /// var restoreTask = NuGet.Restore.FromFeed("myFeed", true);
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: restore
        ///     feedsToUse: myFeed
        ///     includeNuGetOrg: true
        /// </code>
        /// </example>
        public NuGetRestoreBuilder Restore => new();

        /// <summary>
        /// Gets a NuGetPushBuilder instance to create NuGet push tasks.
        /// </summary>
        /// <example>
        /// <code>
        /// var pushTask = NuGet.Push.ToInternalFeed("myInternalFeed");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: push
        ///     publishVstsFeed: myInternalFeed
        /// </code>
        /// </example>
        public NuGetPushBuilder Push => new();

        /// <summary>
        /// Gets a NuGetPackBuilder instance to create NuGet pack tasks.
        /// </summary>
        /// <example>
        /// <code>
        /// var packTask = NuGet.Pack.Pack("**/*.csproj", "-Properties Configuration=Release");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: pack
        ///     packagesToPack: **/*.csproj
        ///     arguments: -Properties Configuration=Release
        /// </code>
        /// </example>
        public NuGetPackBuilder Pack => new();

        /// <summary>
        /// Gets a <see cref="NuGetCustomCommandTask"/> instance to create custom NuGet tasks.
        /// <para>
        /// For example:
        /// </para>
        /// <code>
        /// var customTask = NuGet.Custom(@"config -Set repositoryPath=c:\packages -configfile c:\my.config");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
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
        /// Creates a NuGetRestoreCommandTask to restore packages from a feed.
        /// </summary>
        /// <param name="vstsFeed">The feed to restore packages from.</param>
        /// <param name="includeNuGetOrg">A value indicating whether to include NuGet.org in the generated NuGet.config.</param>
        /// <returns>A NuGetRestoreCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// NuGet.Restore.FromFeed("myFeed") with
        /// {
        ///   RestoreSolution = "path/to/solution.sln"
        /// }
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: restore
        ///     feedsToUse: select
        ///     restoreSolution: path/to/solution.sln
        ///     feedsToUse: myFeed
        /// </code>
        /// </example>
        public NuGetRestoreFeedCommandTask FromFeed(string vstsFeed)
        {
            return new()
            {
                VstsFeed = vstsFeed
            };
        }

        /// <summary>
        /// Creates a NuGetRestoreCommandTask to restore packages from a NuGet.config file.
        /// </summary>
        /// <param name="nugetConfigPath">The path to the NuGet.config file.</param>
        /// <returns>A NuGetRestoreCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// NuGet.Restore.FromNuGetConfig("path/to/NuGet.config");
        /// {
        ///   RestoreSolution = "path/to/solution.sln"
        /// }
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: restore
        ///     feedsToUse: config
        ///     restoreSolution: path/to/solution.sln
        ///     nugetConfigPath: path/to/NuGet.config
        /// </code>
        /// </example>
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
        /// Creates a NuGetPushCommandTask to push packages to an internal feed.
        /// </summary>
        /// <param name="targetFeed">The target feed.</param>
        /// <returns>A NuGetPushCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// var pushTask = NuGet.Push.ToInternalFeed("myInternalFeed");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: push
        ///     nuGetFeedType: internal
        ///     publishVstsFeed: myInternalFeed
        /// </code>
        /// </example>
        public NuGetPushInternalCommandTask ToInternalFeed(string publishVstsFeed)
        {
            return new(publishVstsFeed)
            {
            };
        }

        /// <summary>
        /// Creates a NuGetPushCommandTask to push packages to an external feed.
        /// </summary>
        /// <param name="targetFeedCredentials">The target feed credentials.</param>
        /// <returns>A NuGetPushCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// var pushTask = NuGet.Push.ToExternalFeed("myExternalFeedCredentials");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: push
        ///     nuGetFeedType: external
        ///     externalFeedCredentials: myExternalFeedCredentials
        /// </code>
        /// </example>
        public NuGetPushExternalCommandTask ToExternalFeed(string targetFeedCredentials) => new(targetFeedCredentials);
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
