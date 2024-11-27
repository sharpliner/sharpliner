using System;
using Sharpliner.AzureDevOps.Tasks;

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
        /// Gets a NuGetCustomBuilder instance to create custom NuGet tasks.
        /// </summary>
        /// <example>
        /// <code>
        /// var customTask = NuGet.Custom.CustomCommand("custom", "-CustomArg");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: custom
        ///     arguments: -CustomArg
        /// </code>
        /// </example>
        public NuGetCustomBuilder Custom => new();
    }

    /// <summary>
    /// Represents the NuGetAuthenticate@1 task in Azure DevOps pipelines.
    /// </summary>
    public class NuGetAuthenticateTask : AzureDevOpsTask
    {
        /// <summary>
        /// Gets or sets the NuGet service connections.
        /// </summary>
        public string[]? NuGetServiceConnections { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether to force reinstall the credential provider.
        /// </summary>
        public bool ForceReinstallCredentialProvider { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetAuthenticateTask"/> class.
        /// </summary>
        public NuGetAuthenticateTask() : base("NuGetAuthenticate@1")
        {
        }
    }

    /// <summary>
    /// Provides methods to create NuGet restore tasks.
    /// </summary>
    public class NuGetRestoreBuilder
    {
        /// <summary>
        /// Creates a NuGetRestoreCommandTask to restore packages from a feed.
        /// </summary>
        /// <param name="feed">The feed to restore packages from.</param>
        /// <param name="includeNuGetOrg">A value indicating whether to include NuGet.org in the generated NuGet.config.</param>
        /// <returns>A NuGetRestoreCommandTask instance.</returns>
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
        public NuGetRestoreCommandTask FromFeed(string feed, bool? includeNuGetOrg = null)
        {
            return new NuGetRestoreCommandTask
            {
                Feed = feed,
                IncludeNuGetOrg = includeNuGetOrg
            };
        }

        /// <summary>
        /// Creates a NuGetRestoreCommandTask to restore packages from a NuGet.config file.
        /// </summary>
        /// <param name="nugetConfigPath">The path to the NuGet.config file.</param>
        /// <returns>A NuGetRestoreCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// var restoreTask = NuGet.Restore.FromNuGetConfig("path/to/NuGet.config");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: restore
        ///     nugetConfigPath: path/to/NuGet.config
        /// </code>
        /// </example>
        public NuGetRestoreCommandTask FromNuGetConfig(string nugetConfigPath)
        {
            return new NuGetRestoreCommandTask
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
        ///     publishVstsFeed: myInternalFeed
        /// </code>
        /// </example>
        public NuGetPushCommandTask ToInternalFeed(string targetFeed)
        {
            return new NuGetPushCommandTask
            {
                TargetFeed = targetFeed
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
        ///     externalFeedCredentials: myExternalFeedCredentials
        /// </code>
        /// </example>
        public NuGetPushCommandTask ToExternalFeed(string targetFeedCredentials)
        {
            return new NuGetPushCommandTask
            {
                TargetFeedCredentials = targetFeedCredentials
            };
        }
    }

    /// <summary>
    /// Provides methods to create NuGet pack tasks.
    /// </summary>
    public class NuGetPackBuilder
    {
        /// <summary>
        /// Creates a NuGetPackCommandTask to pack NuGet packages.
        /// </summary>
        /// <param name="packagesToPack">The pattern to search for csproj or nuspec files to pack.</param>
        /// <param name="arguments">The additional arguments for the pack command.</param>
        /// <returns>A NuGetPackCommandTask instance.</returns>
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
        public NuGetPackCommandTask Pack(string packagesToPack = "**/*.csproj", string? arguments = null)
        {
            return new NuGetPackCommandTask
            {
                PackagesToPack = packagesToPack,
                Arguments = arguments
            };
        }
    }

    /// <summary>
    /// Provides methods to create custom NuGet tasks.
    /// </summary>
    public class NuGetCustomBuilder
    {
        /// <summary>
        /// Creates a NuGetCustomCommandTask with the specified command and arguments.
        /// </summary>
        /// <param name="command">The custom command.</param>
        /// <param name="arguments">The additional arguments for the custom command.</param>
        /// <returns>A NuGetCustomCommandTask instance.</returns>
        /// <example>
        /// <code>
        /// var customTask = NuGet.Custom.CustomCommand("custom", "-CustomArg");
        /// </code>
        /// <para>Generated YAML:</para>
        /// <code>
        /// - task: NuGetCommand@2
        ///   inputs:
        ///     command: custom
        ///     arguments: -CustomArg
        /// </code>
        /// </example>
        public NuGetCustomCommandTask CustomCommand(string command, string? arguments = null)
        {
            return new NuGetCustomCommandTask
            {
                Command = command,
                Arguments = arguments
            };
        }
    }

    /// <summary>
    /// Represents a base class for NuGet command tasks in Azure DevOps pipelines.
    /// </summary>
    public abstract class NuGetCommandTask : AzureDevOpsTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetCommandTask"/> class with the specified task name.
        /// </summary>
        /// <param name="taskName">The task name.</param>
        protected NuGetCommandTask(string taskName) : base(taskName)
        {
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for restoring NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public class NuGetRestoreCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Gets or sets the feed to restore packages from.
        /// </summary>
        public string? Feed { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether to include NuGet.org in the generated NuGet.config.
        /// </summary>
        public bool? IncludeNuGetOrg { get; init; }

        /// <summary>
        /// Gets or sets the path to the NuGet.config file.
        /// </summary>
        public string? NuGetConfigPath { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetRestoreCommandTask"/> class.
        /// </summary>
        public NuGetRestoreCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for pushing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public class NuGetPushCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Gets or sets the target feed.
        /// </summary>
        public string? TargetFeed { get; init; }

        /// <summary>
        /// Gets or sets the target feed credentials.
        /// </summary>
        public string? TargetFeedCredentials { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPushCommandTask"/> class.
        /// </summary>
        public NuGetPushCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public class NuGetPackCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Gets or sets the pattern to search for csproj or nuspec files to pack.
        /// </summary>
        public string? PackagesToPack { get; init; }

        /// <summary>
        /// Gets or sets the additional arguments for the pack command.
        /// </summary>
        public string? Arguments { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackCommandTask"/> class.
        /// </summary>
        public NuGetPackCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    /// <summary>
    /// Represents the NuGetCommand@2 task for custom NuGet commands in Azure DevOps pipelines.
    /// </summary>
    public class NuGetCustomCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Gets or sets the custom command.
        /// </summary>
        public string? Command { get; init; }

        /// <summary>
        /// Gets or sets the additional arguments for the custom command.
        /// </summary>
        public string? Arguments { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetCustomCommandTask"/> class.
        /// </summary>
        public NuGetCustomCommandTask() : base("NuGetCommand@2")
        {
        }
    }
}
