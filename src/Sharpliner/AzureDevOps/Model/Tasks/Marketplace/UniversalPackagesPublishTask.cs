using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v0?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// Universal Packages Task when command = "publish"
/// </summary>
public record UniversalPackagesPublishTask : UniversalPackagesTask
{
    /// <summary>
    /// Specifies the path to list of files to be published.
    /// Required when command = publish.
    /// Default value: $(Build.ArtifactStagingDirectory).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishDirectory
    {
        get => GetConditioned<string>("publishDirectory");
        init => SetProperty("publishDirectory", value);
    }

    /// <summary>
    /// Specifies a feed from this collection or another collection in Azure Artifacts.
    /// Required when command = publish.
    /// Allowed values: internal (This organization/collection), external (Another organization/collection).
    /// Default value: internal.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FeedsToUsePublish
    {
        get => GetConditioned<string>("feedsToUsePublish");
        init => SetProperty("feedsToUsePublish", value);
    }

    /// <summary>
    /// Specifies the credentials to use for external feeds.
    /// Required when internalOrExternalPublish = external and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishFeedCredentials
    {
        get => GetConditioned<string>("publishFeedCredentials");
        init => SetProperty("publishFeedCredentials", value);
    }

    /// <summary>
    /// Specifies the project and the feed's name/GUID to publish to.
    /// Required when internalOrExternalPublish = internal and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeedPublish
    {
        get => GetConditioned<string>("vstsFeedPublish");
        init => SetProperty("vstsFeedPublish", value);
    }

    /// <summary>
    /// Associates this build/release pipeline's metadata (such as run # and source code information) with the package.
    /// Optional. Use when command = publish and internalOrExternalPublish = internal.
    /// Default value: true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishPackageMetadata
    {
        get => GetConditioned<string>("publishPackageMetadata");
        init => SetProperty("publishPackageMetadata", value);
    }

    /// <summary>
    /// Specifies a package ID to publish or creates a new package ID if you've never published a version of this package
    ///   before. Package names must be lower case and can only use letters, numbers, and dashes (-).
    /// Required when internalOrExternalPublish = internal and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeedPackagePublish
    {
        get => GetConditioned<string>("vstsFeedPackagePublish");
        init => SetProperty("vstsFeedPackagePublish", value);
    }

    /// <summary>
    /// Specifies the external feed name to publish to. If the feed was created in a project, the value should be Project/Feed,
    ///   where Project is the project's name or ID, and Feed is the feed's name.If the feed was not created in a project, the
    ///   value should be only the feed name.
    /// Required when internalOrExternalPublish = external and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FeedPublishExternal
    {
        get => GetConditioned<string>("feedPublishExternal");
        init => SetProperty("feedPublishExternal", value);
    }

    /// <summary>
    /// Specifies the package name when publishing to an external feed.
    /// Required when internalOrExternalPublish = external and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackagePublishExternal
    {
        get => GetConditioned<string>("packagePublishExternal");
        init => SetProperty("packagePublishExternal", value);
    }

    /// <summary>
    /// Specifies a version increment strategy. The custom value to input your package version manually. For new packages, the
    ///   first version will be 1.0.0 if you specify major, 0.1.0 if you specify minor, or 0.0.1 if you specify patch.
    /// Required when command = publish.
    /// Allowed values: major (Next major), minor (Next minor), patch (Next patch), custom.
    /// Default value: patch.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionOption
    {
        get => GetConditioned<string>("versionOption");
        init => SetProperty("versionOption", value);
    }

    /// <summary>
    /// Specifies a custom version schema for the package.
    /// Required when versionPublishSelector = custom and command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionPublish
    {
        get => GetConditioned<string>("versionPublish");
        init => SetProperty("versionPublish", value);
    }

    /// <summary>
    /// Specifies the description of the package contents and/or the changes made in this version of the package.
    /// Optional. Use when command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackagePublishDescription
    {
        get => GetConditioned<string>("packagePublishDescription");
        init => SetProperty("packagePublishDescription", value);
    }

    /// <summary>
    /// Specifies a name for the variable that will contain the published package name and version.
    /// Optional. Use when command = publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishedPackageVar
    {
        get => GetConditioned<string>("publishedPackageVar");
        init => SetProperty("publishedPackageVar", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesPublishTask"/> class with required properties.
    /// </summary>
    public UniversalPackagesPublishTask()
        : base("publish")
    {
    }
}
