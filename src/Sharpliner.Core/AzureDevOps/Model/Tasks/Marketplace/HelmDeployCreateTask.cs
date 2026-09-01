using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: create</c>, which creates a new chart with the given name.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployCreateTask : HelmDeployCommandTask
{
    /// <summary>
    /// Required <c>string</c> input. The name of the chart to create.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartName
    {
        get => GetExpression<string>("chartName");
        init => SetProperty("chartName", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployCreateTask"/> record.
    /// </summary>
    /// <param name="chartName">The name of the chart to create.</param>
    public HelmDeployCreateTask(AdoExpression<string> chartName) : base("create")
    {
        ChartName = chartName;
    }
}
