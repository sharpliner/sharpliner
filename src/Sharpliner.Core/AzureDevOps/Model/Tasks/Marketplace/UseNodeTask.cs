using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/use-node-v1">UseNode@1</see>
/// task in Azure DevOps pipelines.
/// More details are available in the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/UseNodeV1/task.json">UseNodeV1/task.json</see>
/// task specification.
/// </summary>
public record UseNodeTask : AzureDevOpsTask
{
    /// <summary>
    /// Selects how Node.js version is resolved.
    /// Default value: <see cref="UseNodeVersionSource.Spec"/>.
    /// </summary>
    [YamlIgnore]
    public UseNodeVersionSource VersionSource
    {
        get => GetEnum("versionSource", UseNodeVersionSource.Spec);
        init => SetProperty("versionSource", value);
    }

    /// <summary>
    /// Version spec of Node.js to use (for example <c>10.x</c>, <c>10.15.1</c>, or <c>&gt;=10.15.0</c>).
    /// Applies when <see cref="VersionSource"/> is <see cref="UseNodeVersionSource.Spec"/>.
    /// Default value: <c>10.x</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Version
    {
        get => GetExpression<string>("version", "10.x");
        init => SetProperty("version", value);
    }

    /// <summary>
    /// Path to a version file (for example <c>src/.nvmrc</c>).
    /// Applies when <see cref="VersionSource"/> is <see cref="UseNodeVersionSource.FromFile"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionFilePath
    {
        get => GetExpression<string>("versionFilePath");
        init => SetProperty("versionFilePath", value);
    }

    /// <summary>
    /// Always checks online for the latest version that satisfies the version spec.
    /// Keep this <c>false</c> for better cache reuse on self-hosted agents unless your pipeline requires always fetching the latest patch.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckLatest
    {
        get => GetExpression<bool>("checkLatest", false);
        init => SetProperty("checkLatest", value);
    }

    /// <summary>
    /// Desired Node.js architecture.
    /// Serialized to the task's <c>force32bit</c> input.
    /// Default value: <see cref="NodeArchitecture.X64"/>.
    /// </summary>
    [YamlIgnore]
    public NodeArchitecture Architecture
    {
        get => GetBool("force32bit", false) ? NodeArchitecture.X86 : NodeArchitecture.X64;
        init => SetProperty("force32bit", value == NodeArchitecture.X86);
    }

    /// <summary>
    /// Alternative mirror URL used to download Node.js binaries.
    /// Default value: <c>https://nodejs.org/dist</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? NodejsMirror
    {
        get => GetExpression<string>("nodejsMirror", "https://nodejs.org/dist");
        init => SetProperty("nodejsMirror", value);
    }

    /// <summary>
    /// Number of retries when Node.js binary downloads fail.
    /// Default value: <c>5</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RetryCountOnDownloadFails
    {
        get => GetExpression<int>("retryCountOnDownloadFails", 5);
        init => SetProperty("retryCountOnDownloadFails", value);
    }

    /// <summary>
    /// Delay between download retries in milliseconds.
    /// Default value: <c>1000</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? DelayBetweenRetries
    {
        get => GetExpression<int>("delayBetweenRetries", 1000);
        init => SetProperty("delayBetweenRetries", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseNodeTask"/> class.
    /// </summary>
    public UseNodeTask() : base("UseNode@1")
    {
        DisplayName = "Use Node.js";
    }
}

/// <summary>
/// Source used by <see cref="UseNodeTask"/> to resolve the Node.js version.
/// </summary>
public enum UseNodeVersionSource
{
    /// <summary>
    /// Resolve the version from an explicit version specification.
    /// </summary>
    [YamlMember(Alias = "spec")]
    Spec,

    /// <summary>
    /// Resolve the version from a file such as <c>.nvmrc</c>.
    /// </summary>
    [YamlMember(Alias = "fromFile")]
    FromFile,
}

/// <summary>
/// Node.js architecture preference for <see cref="UseNodeTask"/>.
/// </summary>
public enum NodeArchitecture
{
    /// <summary>
    /// Use 64-bit Node.js binaries.
    /// </summary>
    X64,

    /// <summary>
    /// Use 32-bit Node.js binaries.
    /// </summary>
    X86,
}
