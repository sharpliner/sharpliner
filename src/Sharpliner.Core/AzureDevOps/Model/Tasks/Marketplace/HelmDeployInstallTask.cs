using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: install</c>, which installs a chart into the Kubernetes cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployInstallTask : HelmDeployChartTask
{
    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Runs <c>helm dependency update</c> before installing the chart.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UpdateDependency
    {
        get => GetExpression<bool>("updatedependency");
        init => SetProperty("updatedependency", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployInstallTask"/> record.
    /// </summary>
    public HelmDeployInstallTask() : base("install")
    {
    }
}
