using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base record for the <c>HelmDeploy@1</c> commands that operate on an existing release, i.e. <c>delete</c>, <c>uninstall</c> and <c>rollback</c>.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record HelmDeployReleaseTask : HelmDeployCommandTask
{
    /// <summary>
    /// Optional <c>string</c> input. The name of the release the command is run against.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ReleaseName
    {
        get => GetExpression<string>("releaseName");
        init => SetProperty("releaseName", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployReleaseTask"/> record for the given Helm command.
    /// </summary>
    /// <param name="command">The Helm command to run, e.g. <c>uninstall</c>.</param>
    protected HelmDeployReleaseTask(string command) : base(command)
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: delete</c>, which deletes a release from the cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployDeleteTask : HelmDeployReleaseTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployDeleteTask"/> record.
    /// </summary>
    public HelmDeployDeleteTask() : base("delete")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: uninstall</c>, which uninstalls a release from the cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployUninstallTask : HelmDeployReleaseTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployUninstallTask"/> record.
    /// </summary>
    public HelmDeployUninstallTask() : base("uninstall")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: rollback</c>, which rolls a release back to a previous revision.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployRollbackTask : HelmDeployReleaseTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployRollbackTask"/> record.
    /// </summary>
    public HelmDeployRollbackTask() : base("rollback")
    {
    }
}
