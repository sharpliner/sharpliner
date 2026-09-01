using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base record for the <c>HelmDeploy@1</c> commands that work with a chart, i.e. <c>install</c> and <c>upgrade</c>.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record HelmDeployChartTask : HelmDeployCommandTask
{
    /// <summary>
    /// <para>
    /// Required <c>pickList</c> input. Specifies how the chart is referenced, either by <see cref="ChartName"/> or by <see cref="ChartPath"/>.
    /// </para>
    /// Default value: <see cref="HelmChartType.Name"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<HelmChartType>? ChartType
    {
        get => GetExpression<HelmChartType>("chartType");
        init => SetProperty("chartType", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input. The chart reference to install, which can be a URL or a chart name.
    /// </para>
    /// <para>
    /// For example, when the chart name is <c>stable/mysql</c>, the task runs <c>helm install stable/mysql</c>.
    /// </para>
    /// Required when <see cref="ChartType"/> is <see cref="HelmChartType.Name"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartName
    {
        get => GetExpression<string>("chartName");
        init => SetProperty("chartName", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>filePath</c> input. The path to a packaged chart or to an unpacked chart directory.
    /// </para>
    /// <para>
    /// For example, when <c>./redis</c> is specified, the task runs <c>helm install ./redis</c>.
    /// </para>
    /// Required when <see cref="ChartType"/> is <see cref="HelmChartType.FilePath"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ChartPath
    {
        get => GetExpression<string>("chartPath");
        init => SetProperty("chartPath", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The exact chart version to install. When it is not specified, the latest version is installed.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Version
    {
        get => GetExpression<string>("version");
        init => SetProperty("version", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The release name. When it is not specified, Helm generates one.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ReleaseName
    {
        get => GetExpression<string>("releaseName");
        init => SetProperty("releaseName", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input. Values set on the command line, which the task turns into <c>--set</c> options.
    /// </para>
    /// Multiple values can be separated by commas or newlines, e.g. <c>key1=val1,key2=val2</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideValues
    {
        get => GetExpression<string>("overrideValues");
        init => SetProperty("overrideValues", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>filePath</c> input. Values specified in a YAML file or a URL.
    /// </para>
    /// For example, <c>myvalues.yaml</c> results in <c>helm install --values=myvalues.yaml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ValueFile
    {
        get => GetExpression<string>("valueFile");
        init => SetProperty("valueFile", value);
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
    /// Initializes a new instance of the <see cref="HelmDeployChartTask"/> record for the given Helm command.
    /// </summary>
    /// <param name="command">The Helm command to run, either <c>install</c> or <c>upgrade</c>.</param>
    protected HelmDeployChartTask(string command) : base(command)
    {
    }
}
