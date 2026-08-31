using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the abstract <c>UniversalPackages@1</c> Azure Pipelines task.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v1">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/UniversalPackagesV1/task.json">official UniversalPackagesV1 task specification</see>.
/// </summary>
public abstract record UniversalPackagesV1Task : AzureDevOpsTask
{
    /// <summary>
    /// Required <c>pickList</c> input. Specifies whether to download or publish a Universal Package.
    /// Allowed values: <c>download</c>, <c>publish</c>.
    /// Default value: <c>download</c>.
    /// </summary>
    [YamlIgnore]
    internal AdoExpression<string>? Command
    {
        get => GetExpression<string>("command");
        init => SetProperty("command", value);
    }

    /// <summary>
    /// Required <c>string</c> input. Feed name.
    /// For project-scoped feeds use the <c>project/feed</c> format.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Feed
    {
        get => GetExpression<string>("feed");
        init => SetProperty("feed", value);
    }

    /// <summary>
    /// Required <c>string</c> input. Universal Package name.
    /// Package names must be lowercase and may contain letters, numbers, and dashes (<c>-</c>).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackageName
    {
        get => GetExpression<string>("packageName");
        init => SetProperty("packageName", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. Package version.
    /// Required for download. For publish, either this input or <see cref="UniversalPackagesV1PublishTask.VersionIncrement"/> must be set.
    /// Can include wildcards for downloads, such as <c>*</c>, <c>1.*</c>, or <c>1.2.*</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackageVersion
    {
        get => GetExpression<string>("packageVersion");
        init => SetProperty("packageVersion", value);
    }

    /// <summary>
    /// Required <c>filePath</c> input. For downloads, the destination directory.
    /// For publish, the source directory that contains files to publish.
    /// Default value: <c>$(System.DefaultWorkingDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Directory
    {
        get => GetExpression<string>("directory");
        init => SetProperty("directory", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesV1Task"/> class.
    /// </summary>
    protected UniversalPackagesV1Task(string command, string feed, string packageName)
        : base("UniversalPackages@1")
    {
        Command = Require.NotNullAndNotEmpty(command);
        Feed = Require.NotNullAndNotEmpty(feed);
        PackageName = Require.NotNullAndNotEmpty(packageName);
    }
}

/// <summary>
/// Represents <c>UniversalPackages@1</c> with <c>command: download</c>.
/// </summary>
public record UniversalPackagesV1DownloadTask : UniversalPackagesV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesV1DownloadTask"/> class.
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="packageVersion">Package version to download. Wildcards are supported, such as <c>*</c>, <c>1.*</c>, or <c>1.2.*</c>.</param>
    public UniversalPackagesV1DownloadTask(string feed, string packageName, string packageVersion)
        : base("download", feed, packageName)
    {
        DisplayName = "Download universal package";
        PackageVersion = Require.NotNullAndNotEmpty(packageVersion);
    }
}

/// <summary>
/// Supported values for the <c>versionIncrement</c> input of <see cref="UniversalPackagesV1PublishTask"/>.
/// </summary>
public enum UniversalPackagesV1VersionIncrement
{
    /// <summary>
    /// Increments the major version component.
    /// </summary>
    [YamlMember(Alias = "major")]
    Major,

    /// <summary>
    /// Increments the minor version component.
    /// </summary>
    [YamlMember(Alias = "minor")]
    Minor,

    /// <summary>
    /// Increments the patch version component.
    /// </summary>
    [YamlMember(Alias = "patch")]
    Patch,
}

/// <summary>
/// Represents <c>UniversalPackages@1</c> with <c>command: publish</c>.
/// </summary>
public record UniversalPackagesV1PublishTask : UniversalPackagesV1Task
{
    /// <summary>
    /// Optional <c>pickList</c> input. Automatically increments the package version.
    /// Allowed values: <see cref="UniversalPackagesV1VersionIncrement.Major"/>, <see cref="UniversalPackagesV1VersionIncrement.Minor"/>,
    /// and <see cref="UniversalPackagesV1VersionIncrement.Patch"/>.
    /// Cannot be used together with <see cref="UniversalPackagesV1Task.PackageVersion"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<UniversalPackagesV1VersionIncrement>? VersionIncrement
    {
        get => GetExpression<UniversalPackagesV1VersionIncrement>("versionIncrement");
        init => SetProperty("versionIncrement", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. Description of this package version.
    /// Use when <c>command = publish</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackageDescription
    {
        get => GetExpression<string>("packageDescription");
        init => SetProperty("packageDescription", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesV1PublishTask"/> class that publishes a specific package version.
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="packageVersion">Package version to publish.</param>
    public UniversalPackagesV1PublishTask(string feed, string packageName, string packageVersion)
        : base("publish", feed, packageName)
    {
        DisplayName = "Publish universal package";
        PackageVersion = Require.NotNullAndNotEmpty(packageVersion);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesV1PublishTask"/> class that auto-increments package versions.
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="versionIncrement">Version component to increment automatically.</param>
    public UniversalPackagesV1PublishTask(string feed, string packageName, AdoExpression<UniversalPackagesV1VersionIncrement> versionIncrement)
        : base("publish", feed, packageName)
    {
        DisplayName = "Publish universal package";
        VersionIncrement = versionIncrement;
    }
}
