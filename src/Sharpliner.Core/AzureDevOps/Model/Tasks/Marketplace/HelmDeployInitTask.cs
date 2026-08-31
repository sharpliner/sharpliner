using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: init</c>, which initializes Helm and installs Tiller in the cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployInitTask : HelmDeployCommandTask
{
    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Uses the canary Tiller image, which is the latest pre-release version of Tiller.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CanaryImage
    {
        get => GetExpression<bool>("canaryimage");
        init => SetProperty("canaryimage", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Upgrades Tiller when it is already installed.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UpgradeTiller
    {
        get => GetExpression<bool>("upgradetiller");
        init => SetProperty("upgradetiller", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Blocks until the command execution completes.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? WaitForExecution
    {
        get => GetExpression<bool>("waitForExecution");
        init => SetProperty("waitForExecution", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployInitTask"/> record.
    /// </summary>
    public HelmDeployInitTask() : base("init")
    {
    }
}
