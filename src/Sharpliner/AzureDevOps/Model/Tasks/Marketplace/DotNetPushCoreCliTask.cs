using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>dotnet nuget push</c> command.
/// </summary>
public record DotNetPushCoreCliTask : DotNetCoreCliTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetPushCoreCliTask"/> class.
    /// </summary>
    public DotNetPushCoreCliTask() : base("push")
    {
    }

    /// <summary>
    /// <para>
    /// The pattern to match or path to nupkg files to be uploaded
    /// </para>
    /// <para>
    /// Multiple patterns can be separated by a semicolon, and you can make a pattern negative by prefixing it with !.
    /// </para>
    /// <para>
    /// Example: <c>**/*.nupkg;!**/*.Tests.nupkg</c>.
    /// </para>
    /// <para>
    /// Argument aliases: <c>searchPatternPush</c>
    /// </para>
    /// </summary>
    [YamlIgnore]
    public string? PackagesToPush
    {
        get => GetString("packagesToPush");
        init => SetProperty("packagesToPush", value);
    }

    /// <summary>
    /// Publishes to internal feed
    /// </summary>
    /// <param name="targetFeed">Select a feed hosted in your organization. You must have Package Management installed and licensed to select a feed here</param>
    public DotNetPushCoreCliTask PublishInternally(string targetFeed)
    {
        SetProperty("nuGetFeedType", "internal");
        SetProperty("feedPublish", targetFeed);
        return this;
    }

    /// <summary>
    /// Publishes to external feed
    /// </summary>
    /// <param name="targetFeedCredentials">The NuGet service connection that contains the external NuGet server’s credentials.</param>
    public DotNetPushCoreCliTask PublishExternally(string targetFeedCredentials)
    {
        SetProperty("nuGetFeedType", "external");
        SetProperty("publishFeedCredentials", targetFeedCredentials);
        return this;
    }

    /// <summary>
    /// Associate this build/release pipeline’s metadata (run ID, source code information) with the package
    /// </summary>
    [YamlIgnore]
    public bool PublishPackageMetadata
    {
        get => GetBool("publishPackageMetadata", true);
        init => SetProperty("publishPackageMetadata", value ? "true" : "false");
    }
}
