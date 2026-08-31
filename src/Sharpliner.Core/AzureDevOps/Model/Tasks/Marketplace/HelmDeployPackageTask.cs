using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: package</c>, which packages a chart directory into a chart archive.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployPackageTask : HelmDeployCommandTask
{
    /// <summary>
    /// Required <c>filePath</c> input. The path to the chart directory to package.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartPath
    {
        get => GetExpression<string>("chartPath");
        init => SetProperty("chartPath", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. Sets the version of the chart to this semver version.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Version
    {
        get => GetExpression<string>("version");
        init => SetProperty("version", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input. The destination directory of the packaged chart.
    /// </para>
    /// Default value: <c>$(Build.ArtifactStagingDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Destination
    {
        get => GetExpression<string>("destination");
        init => SetProperty("destination", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Runs <c>helm dependency update</c> before packaging the chart, updating the dependencies
    /// from <c>requirements.yaml</c> to the <c>charts/</c> directory.
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
    /// <para>
    /// Optional <c>boolean</c> input. Saves the packaged chart to the local chart repository.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Save
    {
        get => GetExpression<bool>("save");
        init => SetProperty("save", value);
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
    /// Initializes a new instance of the <see cref="HelmDeployPackageTask"/> record.
    /// </summary>
    public HelmDeployPackageTask() : base("package")
    {
    }
}
