using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the NuGetCommand@2 task for restoring NuGet packages in Azure DevOps pipelines.
/// </summary>
public abstract record NuGetRestoreCommandTask : NuGetCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetRestoreCommandTask"/> class.
    /// </summary>
    protected NuGetRestoreCommandTask(string feedsToUse) : base("restore")
    {
        DisplayName = "NuGet restore";
        FeedsToUse = feedsToUse;
    }

    /// <summary>
    /// Specifies the path to the solution, <c>packages.config</c>, or <c>project.json</c> file that references the packages to be restored.
    /// The official input is <c>solution</c>; <c>restoreSolution</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>**/*.sln</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RestoreSolution
    {
        get => GetExpression<string>("restoreSolution");
        init => SetProperty("restoreSolution", value);
    }

    /// <summary>
    /// Prevents NuGet from using packages from local machine caches when set to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? NoCache
    {
        get => GetExpression<bool>("noCache");
        init => SetProperty("noCache", value);
    }

    /// <summary>
    /// Prevents NuGet from installing multiple packages in parallel when set to <c>true</c>.
    /// Defaults to <c>false</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DisableParallelProcessing
    {
        get => GetExpression<bool>("disableParallelProcessing");
        init => SetProperty("disableParallelProcessing", value);
    }

    /// <summary>
    /// Specifies the folder in which packages are installed.
    /// If no folder is specified, packages are restored into a <c>packages/</c> folder alongside the selected solution,
    /// <c>packages.config</c>, or <c>project.json</c>.
    /// The official input is <c>packagesDirectory</c>; <c>restoreDirectory</c> is the YAML alias emitted for compatibility.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RestoreDirectory
    {
        get => GetExpression<string>("restoreDirectory");
        init => SetProperty("restoreDirectory", value);
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the restore output.
    /// Options are <see cref="NuGetVerbosity.Quiet"/>, <see cref="NuGetVerbosity.Normal"/>, and <see cref="NuGetVerbosity.Detailed"/>.
    /// Defaults to <see cref="NuGetVerbosity.Detailed"/> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<NuGetVerbosity>? VerbosityRestore
    {
        get => GetExpression<NuGetVerbosity>("verbosityRestore");
        init => SetProperty("verbosityRestore", value);
    }

    [YamlIgnore]
    internal AdoExpression<string>? FeedsToUse
    {
        get => GetExpression<string>("feedsToUse");
        init => SetProperty("feedsToUse", value);
    }
}

/// <summary>
/// Represents the NuGetCommand@2 task for restoring NuGet packages with the <c>feedsToUse</c>/<c>selectOrConfig</c> input set to <c>select</c> in Azure DevOps pipelines.
/// </summary>
public record NuGetRestoreFeedCommandTask : NuGetRestoreCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetRestoreFeedCommandTask"/> class.
    /// </summary>
    public NuGetRestoreFeedCommandTask() : base("select")
    {
    }

    /// <summary>
    /// Gets or sets the Azure Artifacts/TFS feed to include in the generated <c>NuGet.config</c>.
    /// The value can be selected from the service feed list or entered as <c>[project name/]feed name</c>.
    /// The official input is <c>feedRestore</c>; <c>vstsFeed</c> is the YAML alias emitted for compatibility.
    /// This input is used when <c>feedsToUse</c>/<c>selectOrConfig</c> is <c>select</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeed
    {
        get => GetExpression<string>("vstsFeed");
        init => SetProperty("vstsFeed", value);
    }

    /// <summary>
    /// Includes NuGet.org in the generated <c>NuGet.config</c> when <c>feedsToUse</c>/<c>selectOrConfig</c> is <c>select</c>.
    /// Defaults to <c>true</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeNuGetOrg
    {
        get => GetExpression<bool>("includeNuGetOrg");
        init => SetProperty("includeNuGetOrg", value);
    }
}

/// <summary>
/// Represents the NuGetCommand@2 task for restoring NuGet packages with the <c>feedsToUse</c>/<c>selectOrConfig</c> input set to <c>config</c> in Azure DevOps pipelines.
/// </summary>
public record NuGetRestoreConfigCommandTask : NuGetRestoreCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetRestoreConfigCommandTask"/> class.
    /// </summary>
    public NuGetRestoreConfigCommandTask() : base("config")
    {
    }

    /// <summary>
    /// Gets or sets the path to the <c>NuGet.config</c> file in the repository that specifies the feeds from which to restore packages.
    /// This input is used when <c>feedsToUse</c>/<c>selectOrConfig</c> is <c>config</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? NuGetConfigPath
    {
        get => GetExpression<string>("nuGetConfigPath");
        init => SetProperty("nuGetConfigPath", value);
    }

    /// <summary>
    /// Specifies the credentials to use for external registries located in the selected <c>NuGet.config</c>.
    /// This is the name of your NuGet service connection. 
    /// For feeds in this organization or collection, leave this blank; the build's credentials are used automatically.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ExternalFeedCredentials
    {
        get => GetExpression<string>("externalFeedCredentials");
        init => SetProperty("externalFeedCredentials", value);
    } 
}
