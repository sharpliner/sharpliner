﻿using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v0?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// Universal Packages Task when command = "download"
/// </summary>
public record UniversalPackagesDownloadTask : UniversalPackagesTask
{
    /// <summary>
    /// Specifies the folder path where the task downloads the package's contents.
    /// Required when command = download.
    /// Default value: $(System.DefaultWorkingDirectory).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DownloadDirectory
    {
        get => GetExpression<string>("downloadDirectory");
        init => SetProperty("downloadDirectory", value);
    }

    /// <summary>
    /// Specifies a feed from this collection or another collection in Azure Artifacts.
    /// Required when command = download.
    /// Allowed values: internal (This organization/collection), external (Another organization/collection).
    /// Default value: internal.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FeedsToUse
    {
        get => GetExpression<string>("feedsToUse");
        init => SetProperty("feedsToUse", value);
    }

    /// <summary>
    /// Specifies the credentials to use for external registries located in the selected NuGet.config. For feeds in this organization or collection, leave this blank; the build's credentials are used automatically.
    /// Optional. Use when feedsToUse = external and command = download
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ExternalFeedCredentials
    {
        get => GetExpression<string>("externalFeedCredentials");
        init => SetProperty("externalFeedCredentials", value);
    }

    /// <summary>
    /// Includes the selected feed. You must have Azure Artifacts installed and licensed to select a feed here. Specifies the FeedName for an organization-scoped feed
    ///   and projectName/FeedName or ProjectID/FeedID for a project-scoped feed.
    /// Required when feedsToUse = internal and command = download
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeed
    {
        get => GetExpression<string>("vstsFeed");
        init => SetProperty("vstsFeed", value);
    }

    /// <summary>
    /// Specifies the name of the package for the task to download.
    /// Required when feedsToUse = internal and command = download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsFeedPackage
    {
        get => GetExpression<string>("vstsFeedPackage");
        init => SetProperty("vstsFeedPackage", value);
    }

    /// <summary>
    /// Specifies the package version or uses a variable containing the version to download. This entry can also be a wildcard expression, such as *, to get the highest
    ///   version. Examples: 1.* gets the highest version with major version 1, and 1.2.* gets the highest patch release with major version 1 and minor version 2.
    /// Required when feedsToUse = internal and command = download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstsPackageVersion
    {
        get => GetExpression<string>("vstsPackageVersion");
        init => SetProperty("vstsPackageVersion", value);
    }

    /// <summary>
    /// Specifies a feed in another organization/collection. For project-scoped feeds, the value should be Project/Feed, where Project is the project's name or ID, and
    ///   Feed is the feed's name/ID.For organization-scoped feeds, the value should be only the feed name.
    /// Required when feedsToUse = external and command = download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FeedDownloadExternal
    {
        get => GetExpression<string>("feedDownloadExternal");
        init => SetProperty("feedDownloadExternal", value);
    }

    /// <summary>
    /// Specifies the package name to download.
    /// Required when feedsToUse = external and command = download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackageDownloadExternal
    {
        get => GetExpression<string>("packageDownloadExternal");
        init => SetProperty("packageDownloadExternal", value);
    }

    /// <summary>
    /// Specifies the package version or uses a variable containing the version to download. This entry can also be a wildcard expression, such as *, to get the highest
    ///   version. Examples: 1.* gets the highest version with major version 1, and 1.2.* gets the highest patch release with major version 1 and minor version 2.
    ///   Wildcard patterns are not supported with pre-release packages.
    /// Required when feedsToUse = external and command = download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionDownloadExternal
    {
        get => GetExpression<string>("versionDownloadExternal");
        init => SetProperty("versionDownloadExternal", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesDownloadTask"/> class with required properties.
    /// </summary>
    public UniversalPackagesDownloadTask()
        : base("download")
    {
    }
}
