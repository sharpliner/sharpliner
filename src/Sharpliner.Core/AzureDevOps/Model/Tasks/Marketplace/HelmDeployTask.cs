using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base record for the <c>HelmDeploy@1</c> Azure Pipelines task, which packages and deploys Helm charts to a Kubernetes cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/HelmDeployV1/task.json">official HelmDeployV1 task specification</see>.
/// <para>
/// Every Helm command is modelled by a dedicated record, e.g. <see cref="HelmDeployInstallTask"/> or <see cref="HelmDeployPackageTask"/>.
/// The <see cref="AzureDevOpsDefinition.Helm"/> builder can be used to create them fluently.
/// </para>
/// </summary>
public abstract record HelmDeployTask : AzureDevOpsTask
{
    /// <summary>
    /// <para>
    /// Required <c>pickList</c> input. Specifies how the task connects to the Kubernetes cluster.
    /// </para>
    /// <para>
    /// Select <see cref="HelmConnectionType.AzureResourceManager"/> to connect to an Azure Kubernetes Service by using an Azure service connection.
    /// Select <see cref="HelmConnectionType.KubernetesServiceConnection"/> to connect to any Kubernetes cluster by using kubeconfig or a service account.
    /// </para>
    /// Default value: <see cref="HelmConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<HelmConnectionType>? ConnectionType
    {
        get => GetExpression<HelmConnectionType>("connectionType");
        init => SetProperty("connectionType", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>connectedService:AzureRM</c> input. The Azure subscription that contains the Azure Kubernetes Service to deploy to.
    /// </para>
    /// Required when <see cref="ConnectionType"/> is <see cref="HelmConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscriptionEndpoint
    {
        get => GetExpression<string>("azureSubscriptionEndpoint");
        init => SetProperty("azureSubscriptionEndpoint", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>pickList</c> input. The Azure resource group that contains the Azure Kubernetes Service.
    /// </para>
    /// Required when <see cref="ConnectionType"/> is <see cref="HelmConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureResourceGroup
    {
        get => GetExpression<string>("azureResourceGroup");
        init => SetProperty("azureResourceGroup", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>pickList</c> input. The name of the Azure Managed Cluster to deploy to.
    /// </para>
    /// Required when <see cref="ConnectionType"/> is <see cref="HelmConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KubernetesCluster
    {
        get => GetExpression<string>("kubernetesCluster");
        init => SetProperty("kubernetesCluster", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Uses cluster administrator credentials instead of the default cluster user credentials.
    /// </para>
    /// <para>
    /// Use when <see cref="ConnectionType"/> is <see cref="HelmConnectionType.AzureResourceManager"/>.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseClusterAdmin
    {
        get => GetExpression<bool>("useClusterAdmin");
        init => SetProperty("useClusterAdmin", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>connectedService:kubernetes</c> input. The Kubernetes service connection to use.
    /// </para>
    /// Required when <see cref="ConnectionType"/> is <see cref="HelmConnectionType.KubernetesServiceConnection"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KubernetesServiceEndpoint
    {
        get => GetExpression<string>("kubernetesServiceEndpoint");
        init => SetProperty("kubernetesServiceEndpoint", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The Kubernetes namespace to use. The Tiller namespace can be set through <see cref="HelmDeployCommandTask.TillerNamespace"/>
    /// or by passing the <c>--tiller-namespace</c> option as an argument.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Namespace
    {
        get => GetExpression<string>("namespace");
        init => SetProperty("namespace", value);
    }

    /// <summary>
    /// Optional <c>connectedService:AzureRM</c> input. The Azure subscription that contains the Azure Container Registry used for Helm charts.
    /// Used with the <c>login</c>, <c>package</c> and <c>push</c> commands.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscriptionEndpointForACR
    {
        get => GetExpression<string>("azureSubscriptionEndpointForACR");
        init => SetProperty("azureSubscriptionEndpointForACR", value);
    }

    /// <summary>
    /// Optional <c>pickList</c> input. The Azure resource group that contains the Azure Container Registry used for Helm charts.
    /// Used with the <c>login</c>, <c>package</c> and <c>push</c> commands.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureResourceGroupForACR
    {
        get => GetExpression<string>("azureResourceGroupForACR");
        init => SetProperty("azureResourceGroupForACR", value);
    }

    /// <summary>
    /// Optional <c>pickList</c> input. The Azure Container Registry that is used for pushing Helm charts.
    /// Used with the <c>login</c>, <c>package</c> and <c>push</c> commands.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureContainerRegistry
    {
        get => GetExpression<string>("azureContainerRegistry");
        init => SetProperty("azureContainerRegistry", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Specifies whether the task fails when anything is written to the standard error stream.
    /// </para>
    /// <para>
    /// When set to <c>false</c>, the task relies on the exit code to determine failure.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnStderr
    {
        get => GetExpression<bool>("failOnStderr");
        init => SetProperty("failOnStderr", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Specifies whether the task collects and publishes deployment metadata.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishPipelineMetadata
    {
        get => GetExpression<bool>("publishPipelineMetadata");
        init => SetProperty("publishPipelineMetadata", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployTask"/> record for the given Helm command.
    /// </summary>
    /// <param name="command">The Helm command to run, e.g. <c>install</c>.</param>
    protected HelmDeployTask(string command)
        : base("HelmDeploy@1")
    {
        SetProperty("command", command);
    }
}

/// <summary>
/// Base record for the <c>HelmDeploy@1</c> commands that run against a Kubernetes cluster and therefore support
/// additional Helm command line options and TLS settings.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record HelmDeployCommandTask : HelmDeployTask
{
    /// <summary>
    /// Optional <c>multiLine</c> input. Additional Helm command options, e.g. <c>--dry-run</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. Enables using SSL between Helm and Tiller.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? EnableTls
    {
        get => GetExpression<bool>("enableTls");
        init => SetProperty("enableTls", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>secureFile</c> input. The CA certificate used to issue the certificate for Tiller and the Helm client.
    /// </para>
    /// Required when <see cref="EnableTls"/> is <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CaCert
    {
        get => GetExpression<string>("caCert");
        init => SetProperty("caCert", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>secureFile</c> input. The Tiller certificate or the Helm client certificate.
    /// </para>
    /// Required when <see cref="EnableTls"/> is <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Certificate
    {
        get => GetExpression<string>("certificate");
        init => SetProperty("certificate", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>secureFile</c> input. The Tiller key or the Helm client key.
    /// </para>
    /// Required when <see cref="EnableTls"/> is <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PrivateKey
    {
        get => GetExpression<string>("privatekey");
        init => SetProperty("privatekey", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The Kubernetes namespace of Tiller.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TillerNamespace
    {
        get => GetExpression<string>("tillernamespace");
        init => SetProperty("tillernamespace", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployCommandTask"/> record for the given Helm command.
    /// </summary>
    /// <param name="command">The Helm command to run, e.g. <c>install</c>.</param>
    protected HelmDeployCommandTask(string command) : base(command)
    {
    }
}

/// <summary>
/// Supported values for the <c>connectionType</c> input of the <c>HelmDeploy@1</c> task.
/// </summary>
public enum HelmConnectionType
{
    /// <summary>
    /// Connect to an Azure Kubernetes Service by using an Azure Resource Manager service connection.
    /// </summary>
    [YamlMember(Alias = "Azure Resource Manager")]
    AzureResourceManager,

    /// <summary>
    /// Connect to any Kubernetes cluster by using a Kubernetes service connection (kubeconfig or service account).
    /// </summary>
    [YamlMember(Alias = "Kubernetes Service Connection")]
    KubernetesServiceConnection,

    /// <summary>
    /// Do not connect to any cluster, e.g. when only packaging a chart.
    /// </summary>
    [YamlMember(Alias = "None")]
    None,
}

/// <summary>
/// Supported values for the <c>chartType</c> input of the <c>HelmDeploy@1</c> task.
/// </summary>
public enum HelmChartType
{
    /// <summary>
    /// The chart is referenced by its name, e.g. <c>stable/mysql</c>.
    /// </summary>
    [YamlMember(Alias = "Name")]
    Name,

    /// <summary>
    /// The chart is referenced by a path to a packaged chart or to an unpacked chart directory.
    /// </summary>
    [YamlMember(Alias = "FilePath")]
    FilePath,
}
