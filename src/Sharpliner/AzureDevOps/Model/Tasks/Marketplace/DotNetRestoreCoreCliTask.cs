using System;
using YamlDotNet.Serialization;
using static Sharpliner.AzureDevOps.Tasks.DotNetTaskBuilder;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet restore` command.
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

    public DotNetRestoreCoreCliTask() : base("restore")
    {
    }

    /// <summary>
    /// Specifies the folder in which packages are installed. If no folder is specified, packages are restored into the default NuGet package cache
    ///
    /// Argument aliases: packagesDirectory
    /// </summary>
    [YamlIgnore]
    public string? RestoreDirectory
    {
        get => GetString(RestoreDirectoryProperty);
        init => SetProperty(RestoreDirectoryProperty, value);
    }

    /// <summary>
    /// Write the additional arguments to be passed to the restore command.
    /// </summary>
    [YamlIgnore]
    public string? RestoreArguments
    {
        get => GetString(RestoreArgumentsProperty);
        init => SetProperty(RestoreArgumentsProperty, value);
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the output for the restore command.
    /// quiet, minimal, normal, detailed, diagnostic
    /// </summary>
    [YamlIgnore]
    public BuildVerbosity VerbosityRestore
    {
        get => GetString(VerbosityRestoreProperty) switch
        {
            "quiet" => BuildVerbosity.Quiet,
            "minimal" => BuildVerbosity.Minimal,
            "normal" => BuildVerbosity.Normal,
            "detailed" => BuildVerbosity.Detailed,
            "diagnostic" => BuildVerbosity.Diagnostic,
            _ => throw new NotImplementedException(),
        };
        init => SetProperty(VerbosityRestoreProperty, value switch
        {
            BuildVerbosity.Quiet => "quiet",
            BuildVerbosity.Minimal => "minimal",
            BuildVerbosity.Normal => "normal",
            BuildVerbosity.Detailed => "detailed",
            BuildVerbosity.Diagnostic => "diagnostic",
            _ => throw new NotImplementedException(),
        });
    }

    /// <summary>
    /// Prevents NuGet from using packages from local machine caches
    /// </summary>
    [YamlIgnore]
    public bool NoCache
    {
        get => GetBool(NoCacheProperty, false);
        init => SetProperty(NoCacheProperty, value ? "true" : "false");
    }

    /// <summary>
    /// Include NuGet.org in the generated NuGet.config000 0. 
    /// </summary>
    [YamlIgnore]
    public bool? IncludeNuGetOrg
    {
        get => GetBool(IncludeNuGetOrgProperty, false);
        init => SetProperty(IncludeNuGetOrgProperty, value.HasValue ? (value.Value ? "true" : "false") : null);
    }

    /// <summary>
    /// The NuGet.config in your repository that specifies the feeds from which to restore packages.
    /// </summary>
    [YamlIgnore]
    public string? NuGetConfigPath
    {
        get => GetString(NugetConfigPathProperty);
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
    /// For feeds in this organization/collection, leave this blank; the build’s credentials are used automatically
    ///
    /// Argument aliases: externalEndpoints
    /// </summary>
    [YamlIgnore]
    public string? ExternalFeedCredentials
    {
        get => GetString(ExternalFeedCredentialsProperty);
        init => SetProperty(ExternalFeedCredentialsProperty, value);
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
