using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: upgrade</c>, which upgrades a release in the Kubernetes cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployUpgradeTask : HelmDeployChartTask
{
    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Installs the release when a release with the given name does not exist yet.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Install
    {
        get => GetExpression<bool>("install");
        init => SetProperty("install", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Restarts the pods of the resource when applicable.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Recreate
    {
        get => GetExpression<bool>("recreate");
        init => SetProperty("recreate", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Resets the values to the ones built into the chart.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ResetValues
    {
        get => GetExpression<bool>("resetValues");
        init => SetProperty("resetValues", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Forces the resource update through a delete/recreate when needed.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Force
    {
        get => GetExpression<bool>("force");
        init => SetProperty("force", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployUpgradeTask"/> record.
    /// </summary>
    public HelmDeployUpgradeTask() : base("upgrade")
    {
    }
}
