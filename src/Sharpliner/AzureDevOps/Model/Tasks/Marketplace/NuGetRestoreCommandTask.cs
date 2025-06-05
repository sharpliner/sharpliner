using Sharpliner.AzureDevOps.ConditionedExpressions;
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
        FeedsToUse = feedsToUse;
    }

    /// <summary>
    /// Specifies the path to the solution, <c>packages.config</c>, or <c>project.json</c> file that references the packages to be restored.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RestoreSolution
    {
        get => GetConditioned<string>("restoreSolution");
        init => SetProperty("restoreSolution", value);
    }

    /// <summary>
    /// Prevents NuGet from using packages from local machine caches when set to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? NoCache
    {
        get => GetConditioned<bool>("noCache");
        init => SetProperty("noCache", value);
    }

    [YamlIgnore]
    internal AdoExpression<string>? FeedsToUse
    {
        get => GetConditioned<string>("feedsToUse");
        init => SetProperty("feedsToUse", value);
    }
}

/// <summary>
/// Represents the NuGetCommand@2 task for restoring NuGet packages with the <c>feedsToUse</c> set to <c>config</c> in Azure DevOps pipelines.
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
    /// Gets or sets the vstsFeed to restore packages from.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeed
    {
        get => GetConditioned<string>("vstsFeed");
        init => SetProperty("vstsFeed", value);
    }

    /// <summary>
    /// Includes NuGet.org in the generated <c>NuGet.config</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeNuGetOrg
    {
        get => GetConditioned<bool>("includeNuGetOrg");
        init => SetProperty("includeNuGetOrg", value);
    }
}

/// <summary>
/// Represents the NuGetCommand@2 task for restoring NuGet packages with the <c>feedsToUse</c> set to <c>config</c> in Azure DevOps pipelines.
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
    /// Gets or sets the path to the NuGet.config file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? NuGetConfigPath
    {
        get => GetConditioned<string>("nuGetConfigPath");
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
        get => GetConditioned<string>("externalFeedCredentials");
        init => SetProperty("externalFeedCredentials", value);
    } 
}
