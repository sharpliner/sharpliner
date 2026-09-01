using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the NuGet tool installer task in Azure DevOps pipelines.
/// This task acquires a specific version of NuGet and adds it to PATH for subsequent tasks.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/nuget-tool-installer-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the task specifications:
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/NuGetToolInstallerV1/task.json">NuGetToolInstallerV1 task.json</see>,
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/NuGetToolInstallerV0/task.json">NuGetToolInstallerV0 task.json</see>.
/// </summary>
public abstract record NuGetToolInstallerTask : AzureDevOpsTask
{
    /// <summary>
    /// Optional. Version or version range of NuGet.exe to install (for example <c>4.x</c>, <c>3.3.x</c>, or <c>>=4.0.0-0</c>).
    /// If unspecified, Azure Pipelines chooses a version automatically.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionSpec
    {
        get => GetExpression<string>("versionSpec");
        init => SetProperty("versionSpec", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. When true, always checks for and downloads the latest available version
    /// that satisfies <see cref="VersionSpec"/> instead of reusing an already cached version.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckLatest
    {
        get => GetExpression<bool>("checkLatest", false);
        init => SetProperty("checkLatest", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetToolInstallerTask"/> class.
    /// </summary>
    /// <param name="taskVersion">The concrete task reference, such as <c>NuGetToolInstaller@1</c>.</param>
    /// <param name="versionSpec">Optional version or version range of NuGet.exe to install.</param>
    /// <param name="checkLatest">Optional value indicating whether to always check for the latest matching version.</param>
    protected NuGetToolInstallerTask(
        string taskVersion,
        AdoExpression<string>? versionSpec = null,
        AdoExpression<bool>? checkLatest = null) : base(taskVersion)
    {
        DisplayName = "Use NuGet";
        VersionSpec = versionSpec;
        CheckLatest = checkLatest;
    }
}

/// <summary>
/// Represents <c>NuGetToolInstaller@1</c> in Azure DevOps pipelines.
/// </summary>
public record NuGetToolInstallerV1Task : NuGetToolInstallerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetToolInstallerV1Task"/> class.
    /// </summary>
    /// <param name="versionSpec">Optional version or version range of NuGet.exe to install.</param>
    /// <param name="checkLatest">Optional value indicating whether to always check for the latest matching version.</param>
    public NuGetToolInstallerV1Task(AdoExpression<string>? versionSpec = null, AdoExpression<bool>? checkLatest = null)
        : base("NuGetToolInstaller@1", versionSpec, checkLatest)
    {
    }
}

/// <summary>
/// Represents <c>NuGetToolInstaller@0</c> in Azure DevOps pipelines.
/// </summary>
public record NuGetToolInstallerV0Task : NuGetToolInstallerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetToolInstallerV0Task"/> class.
    /// </summary>
    /// <param name="versionSpec">Optional version or version range of NuGet.exe to install.</param>
    /// <param name="checkLatest">Optional value indicating whether to always check for the latest matching version.</param>
    public NuGetToolInstallerV0Task(AdoExpression<string>? versionSpec = null, AdoExpression<bool>? checkLatest = null)
        : base("NuGetToolInstaller@0", versionSpec, checkLatest)
    {
    }
}
