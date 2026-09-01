using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: push</c>, which pushes a chart to a remote repository
/// such as an Azure Container Registry.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployPushTask : HelmDeployCommandTask
{
    /// <summary>
    /// Required <c>filePath</c> input. The path to the chart to push.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartPath
    {
        get => GetExpression<string>("chartPath");
        init => SetProperty("chartPath", value);
    }

    /// <summary>
    /// Required <c>string</c> input. The remote repository the chart is pushed to.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RemoteRepo
    {
        get => GetExpression<string>("remoteRepo");
        init => SetProperty("remoteRepo", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The chart name under which the chart is stored in the Azure Container Registry.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartNameForACR
    {
        get => GetExpression<string>("chartNameForACR");
        init => SetProperty("chartNameForACR", value);
    }

    /// <summary>
    /// Optional <c>filePath</c> input. The path to the chart directory used with an Azure Container Registry.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartPathForACR
    {
        get => GetExpression<string>("chartPathForACR");
        init => SetProperty("chartPathForACR", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployPushTask"/> record.
    /// </summary>
    public HelmDeployPushTask() : base("push")
    {
    }
}
