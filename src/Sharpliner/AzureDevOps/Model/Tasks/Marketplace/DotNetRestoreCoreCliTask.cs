using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;
using static Sharpliner.AzureDevOps.Tasks.DotNetTaskBuilder;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>dotnet restore</c> command.
/// </summary>
public record DotNetRestoreCoreCliTask : DotNetCoreCliTask
{
    internal const string FeedsToUseProperty = "feedsToUse";
    internal const string FeedRestoreProperty = "feedRestore";
    internal const string NugetConfigPathProperty = "nugetConfigPath";
    internal const string ConfigRestoreValue = "config";
    internal const string FeedsRestoreValue = "select";

    private const string RestoreDirectoryProperty = "restoreDirectory";
    private const string RestoreArgumentsProperty = "restoreArguments";
    private const string VerbosityRestoreProperty = "verbosityRestore";
    private const string ExternalFeedCredentialsProperty = "externalFeedCredentials";
    private const string NoCacheProperty = "noCache";
    private const string IncludeNuGetOrgProperty = "includeNuGetOrg";

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetRestoreCoreCliTask"/> class.
    /// </summary>
    public DotNetRestoreCoreCliTask() : base("restore")
    {
    }

    /// <summary>
    /// Specifies the folder in which packages are installed. If no folder is specified, packages are restored into the default NuGet package cache
    ///
    /// Argument aliases: packagesDirectory
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? RestoreDirectory
    {
        get => GetConditioned<string>(RestoreDirectoryProperty);
        init => SetProperty(RestoreDirectoryProperty, value);
    }

    /// <summary>
    /// Write the additional arguments to be passed to the restore command.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? RestoreArguments
    {
        get => GetConditioned<string>(RestoreArgumentsProperty);
        init => SetProperty(RestoreArgumentsProperty, value);
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the output for the restore command.
    /// quiet, minimal, normal, detailed, diagnostic
    /// </summary>
    [YamlIgnore]
    public Conditioned<BuildVerbosity>? VerbosityRestore
    {
        get => GetConditioned<BuildVerbosity>(VerbosityRestoreProperty);
        init => SetProperty(VerbosityRestoreProperty, value);
    }

    /// <summary>
    /// Prevents NuGet from using packages from local machine caches
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? NoCache
    {
        get => GetConditioned<bool>(NoCacheProperty);
        init => SetProperty(NoCacheProperty, value);
    }

    /// <summary>
    /// Include NuGet.org in the generated NuGet.config000 0.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? IncludeNuGetOrg
    {
        get => GetConditioned<bool>(IncludeNuGetOrgProperty);
        init => SetProperty(IncludeNuGetOrgProperty, value);
    }

    /// <summary>
    /// The NuGet.config in your repository that specifies the feeds from which to restore packages.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? NuGetConfigPath
    {
        get => GetConditioned<string>(NugetConfigPathProperty);
        init
        {
            var feedsToUse = GetString(FeedsToUseProperty);

            if (!string.IsNullOrEmpty(feedsToUse) && feedsToUse != ConfigRestoreValue)
            {
                throw new Exception(
                    $"Using {NugetConfigPathProperty} requires `{FeedsToUseProperty}` set to `{ConfigRestoreValue}`. " +
                    $"Please use DotNet.Restore.{nameof(DotNetRestoreBuilder.FromNuGetConfig)}() instead of DotNet.Restore.{nameof(DotNetRestoreBuilder.FromFeed)}()");
            }

            SetProperty(NugetConfigPathProperty, value);
            SetProperty(FeedsToUseProperty, ConfigRestoreValue);
        }
    }

    /// <summary>
    /// Credentials to use for external registries located in the selected NuGet.config.
    /// For feeds in this organization/collection, leave this blank; the build's credentials are used automatically
    ///
    /// Argument aliases: externalEndpoints
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? ExternalFeedCredentials
    {
        get => GetConditioned<string>(ExternalFeedCredentialsProperty);
        init => SetProperty(ExternalFeedCredentialsProperty, value);
    }
}

/// <summary>
/// The msbuild verbosity level. See <see cref="Microsoft.Build.Framework.LoggerVerbosity"/>.
/// </summary>
public enum BuildVerbosity
{
    /// <summary>
    /// The most minimal output
    /// </summary>
    [YamlMember(Alias = "quiet")]
    Quiet,

    /// <summary>
    /// Relatively little output
    /// </summary>
    [YamlMember(Alias = "minimal")]
    Minimal,

    /// <summary>
    /// Standard output. This should be the default if verbosity level is not set
    /// </summary>
    [YamlMember(Alias = "normal")]
    Normal,

    /// <summary>
    /// Relatively verbose, but not exhaustive
    /// </summary>
    [YamlMember(Alias = "detailed")]
    Detailed,

    /// <summary>
    /// The most verbose and informative verbosity
    /// </summary>
    [YamlMember(Alias = "diagnostic")]
    Diagnostic,
}
