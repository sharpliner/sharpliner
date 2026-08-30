using System;
using Sharpliner.AzureDevOps.Expressions;
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
        DisplayName = "dotnet restore";
    }

    /// <summary>
    /// Specifies the folder in which packages are installed. If no folder is specified, packages are restored into the default NuGet package cache
    ///
    /// DotNetCoreCLI@2 input: <c>packagesDirectory</c>; serialized using official alias <c>restoreDirectory</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RestoreDirectory
    {
        get => GetExpression<string>(RestoreDirectoryProperty);
        init => SetProperty(RestoreDirectoryProperty, value);
    }

    /// <summary>
    /// Write the additional arguments to be passed to the restore command.
    /// DotNetCoreCLI@2 supports <c>arguments</c> for build, publish, run, test, and custom commands only; restore uses this input instead.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RestoreArguments
    {
        get => GetExpression<string>(RestoreArgumentsProperty);
        init => SetProperty(RestoreArgumentsProperty, value);
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the output for the restore command.
    /// DotNetCoreCLI@2 accepts <c>-</c>, <c>Quiet</c>, <c>Minimal</c>, <c>Normal</c>, <c>Detailed</c>, and <c>Diagnostic</c>.
    /// Default: <c>Normal</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<BuildVerbosity>? VerbosityRestore
    {
        get => GetExpression<BuildVerbosity>(VerbosityRestoreProperty);
        init => SetProperty(VerbosityRestoreProperty, value);
    }

    /// <summary>
    /// Prevents NuGet from using packages from local machine caches.
    /// DotNetCoreCLI@2 defaults this input to false.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? NoCache
    {
        get => GetExpression<bool>(NoCacheProperty);
        init => SetProperty(NoCacheProperty, value);
    }

    /// <summary>
    /// Include NuGet.org in the generated NuGet.config when selected feeds are used.
    /// DotNetCoreCLI@2 defaults this input to true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeNuGetOrg
    {
        get => GetExpression<bool>(IncludeNuGetOrgProperty);
        init => SetProperty(IncludeNuGetOrgProperty, value);
    }

    /// <summary>
    /// The NuGet.config in your repository that specifies the feeds from which to restore packages.
    /// Setting this property selects the <c>config</c> feed mode.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? NuGetConfigPath
    {
        get => GetExpression<string>(NugetConfigPathProperty);
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
    /// DotNetCoreCLI@2 input: <c>externalEndpoints</c>; serialized using official alias <c>externalFeedCredentials</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ExternalFeedCredentials
    {
        get => GetExpression<string>(ExternalFeedCredentialsProperty);
        init => SetProperty(ExternalFeedCredentialsProperty, value);
    }
}

/// <summary>
/// The msbuild verbosity level. See <c>Microsoft.Build.Framework.LoggerVerbosity</c>.
/// </summary>
public enum BuildVerbosity
{
    /// <summary>
    /// The most minimal output
    /// </summary>
    [YamlMember(Alias = "Quiet")]
    Quiet = 0,

    /// <summary>
    /// Relatively little output
    /// </summary>
    [YamlMember(Alias = "Minimal")]
    Minimal = 1,

    /// <summary>
    /// Standard output. This should be the default if verbosity level is not set
    /// </summary>
    [YamlMember(Alias = "Normal")]
    Normal = 2,

    /// <summary>
    /// Relatively verbose, but not exhaustive
    /// </summary>
    [YamlMember(Alias = "Detailed")]
    Detailed = 3,

    /// <summary>
    /// The most verbose and informative verbosity
    /// </summary>
    [YamlMember(Alias = "Diagnostic")]
    Diagnostic = 4,

    /// <summary>
    /// Use the task default verbosity.
    /// </summary>
    [YamlMember(Alias = "-")]
    Default = 5,
}
