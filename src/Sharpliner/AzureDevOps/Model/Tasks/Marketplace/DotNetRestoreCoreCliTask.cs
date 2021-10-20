using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet restore` command.
/// </summary>
public record DotNetRestoreCoreCliTask : DotNetCoreCliTask
{
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
        get => GetString("restoreDirectory");
        init => SetProperty("restoreDirectory", value);
    }

    /// <summary>
    /// Write the additional arguments to be passed to the restore command.
    /// </summary>
    [YamlIgnore]
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
    /// Include NuGet.org in the generated NuGet.config000 0. 
    /// </summary>
    [YamlIgnore]
    public bool? IncludeNuGetOrg
    {
        get => GetBool("includeNuGetOrg", false);
        init => SetProperty("includeNuGetOrg", value.HasValue ? (value.Value ? "true" : "false") : null);
    }

    /// <summary>
    /// The NuGet.config in your repository that specifies the feeds from which to restore packages.
    /// </summary>
    [YamlIgnore]
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
