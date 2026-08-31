using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base type for the Kubectl Azure Pipelines tasks. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/kubernetes-v1?view=azure-pipelines">Kubernetes@1 reference</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/KubernetesV1/task.json">task specification</see>.
/// </summary>
public abstract record KubernetesTask : AzureDevOpsTask
{
    /// <summary>Namespace for the kubectl command.</summary>
    [YamlIgnore] public AdoExpression<string>? Namespace { get => GetExpression<string>("namespace"); init => SetProperty("namespace", value); }
    /// <summary>Arguments passed to the kubectl command.</summary>
    [YamlIgnore] public AdoExpression<string>? Arguments { get => GetExpression<string>("arguments"); init => SetProperty("arguments", value); }
    /// <summary>Whether to use a Kubernetes configuration.</summary>
    [YamlIgnore] public AdoExpression<bool>? UseConfigurationFile { get => GetExpression<bool>("useConfigurationFile"); init => SetProperty("useConfigurationFile", value); }
    /// <summary>Type of Kubernetes configuration.</summary>
    [YamlIgnore] public AdoExpression<KubernetesConfigurationType>? ConfigurationType { get => GetExpression<KubernetesConfigurationType>("configurationType"); init => SetProperty("configurationType", value); }
    /// <summary>Path, directory, or URL of the Kubernetes configuration.</summary>
    [YamlIgnore] public AdoExpression<string>? Configuration { get => GetExpression<string>("configuration"); init => SetProperty("configuration", value); }
    /// <summary>Inline Kubernetes configuration.</summary>
    [YamlIgnore] public AdoExpression<string>? InlineConfiguration { get => GetExpression<string>("inline"); init => SetProperty("inline", value); }
    /// <summary>Type of secret to create or update.</summary>
    [YamlIgnore] public AdoExpression<KubernetesSecretType>? SecretType { get => GetExpression<KubernetesSecretType>("secretType"); init => SetProperty("secretType", value); }
    /// <summary>Arguments used to create a generic secret.</summary>
    [YamlIgnore] public AdoExpression<string>? SecretArguments { get => GetExpression<string>("secretArguments"); init => SetProperty("secretArguments", value); }
    /// <summary>Container registry type for a docker registry secret.</summary>
    [YamlIgnore] public AdoExpression<KubernetesContainerRegistryType>? ContainerRegistryType { get => GetExpression<KubernetesContainerRegistryType>("containerRegistryType"); init => SetProperty("containerRegistryType", value); }
    /// <summary>Docker registry service connection for a docker registry secret.</summary>
    [YamlIgnore] public AdoExpression<string>? DockerRegistryEndpoint { get => GetExpression<string>("dockerRegistryEndpoint"); init => SetProperty("dockerRegistryEndpoint", value); }
    /// <summary>Azure Resource Manager service connection for an Azure Container Registry secret.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureSubscriptionEndpointForSecrets { get => GetExpression<string>("azureSubscriptionEndpointForSecrets"); init => SetProperty("azureSubscriptionEndpointForSecrets", value); }
    /// <summary>Azure Container Registry used for a docker registry secret.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureContainerRegistry { get => GetExpression<string>("azureContainerRegistry"); init => SetProperty("azureContainerRegistry", value); }
    /// <summary>Name of the secret to create or update.</summary>
    [YamlIgnore] public AdoExpression<string>? SecretName { get => GetExpression<string>("secretName"); init => SetProperty("secretName", value); }
    /// <summary>Whether an existing secret is replaced.</summary>
    [YamlIgnore] public AdoExpression<bool>? ForceUpdate { get => GetExpression<bool>("forceUpdate"); init => SetProperty("forceUpdate", value); }
    /// <summary>Name of the ConfigMap to create or update.</summary>
    [YamlIgnore] public AdoExpression<string>? ConfigMapName { get => GetExpression<string>("configMapName"); init => SetProperty("configMapName", value); }
    /// <summary>Whether an existing ConfigMap is replaced.</summary>
    [YamlIgnore] public AdoExpression<bool>? ForceUpdateConfigMap { get => GetExpression<bool>("forceUpdateConfigMap"); init => SetProperty("forceUpdateConfigMap", value); }
    /// <summary>Whether the ConfigMap is read from a file.</summary>
    [YamlIgnore] public AdoExpression<bool>? UseConfigMapFile { get => GetExpression<bool>("useConfigMapFile"); init => SetProperty("useConfigMapFile", value); }
    /// <summary>File or directory containing ConfigMap data.</summary>
    [YamlIgnore] public AdoExpression<string>? ConfigMapFile { get => GetExpression<string>("configMapFile"); init => SetProperty("configMapFile", value); }
    /// <summary>Arguments used to create a ConfigMap without a file.</summary>
    [YamlIgnore] public AdoExpression<string>? ConfigMapArguments { get => GetExpression<string>("configMapArguments"); init => SetProperty("configMapArguments", value); }
    /// <summary>Whether kubectl is selected by version or an explicit path.</summary>
    [YamlIgnore] public AdoExpression<KubectlLocationType>? VersionOrLocation { get => GetExpression<KubectlLocationType>("versionOrLocation"); init => SetProperty("versionOrLocation", value); }
    /// <summary>kubectl version specification.</summary>
    [YamlIgnore] public AdoExpression<string>? VersionSpec { get => GetExpression<string>("versionSpec"); init => SetProperty("versionSpec", value); }
    /// <summary>Whether to check for the latest matching kubectl version.</summary>
    [YamlIgnore] public AdoExpression<bool>? CheckLatest { get => GetExpression<bool>("checkLatest"); init => SetProperty("checkLatest", value); }
    /// <summary>Full path to kubectl.</summary>
    [YamlIgnore] public AdoExpression<string>? SpecifyLocation { get => GetExpression<string>("specifyLocation"); init => SetProperty("specifyLocation", value); }
    /// <summary>Working directory for kubectl.</summary>
    [YamlIgnore] public AdoExpression<string>? WorkingDirectory { get => GetExpression<string>("cwd"); init => SetProperty("cwd", value); }
    /// <summary>Format of the kubectl command output.</summary>
    [YamlIgnore] public AdoExpression<KubernetesOutputFormat>? OutputFormat { get => GetExpression<KubernetesOutputFormat>("outputFormat"); init => SetProperty("outputFormat", value); }

    /// <summary>Initializes a Kubernetes task.</summary>
    protected KubernetesTask(string task) : base(task) { }
}

/// <summary>Models the current <c>Kubernetes@1</c> Kubectl task.</summary>
public abstract record KubernetesV1Task : KubernetesTask
{
    /// <summary>kubectl command to run.</summary>
    [YamlIgnore] public AdoExpression<KubernetesCommand>? Command { get => GetExpression<KubernetesCommand>("command"); init => SetProperty("command", value); }
    /// <summary>Service connection type.</summary>
    [YamlIgnore] public AdoExpression<KubernetesConnectionType>? ConnectionType { get => GetExpression<KubernetesConnectionType>("connectionType"); init => SetProperty("connectionType", value); }

    /// <summary>Initializes a Kubernetes@1 task with its command and connection type.</summary>
    protected KubernetesV1Task(KubernetesCommand command, KubernetesConnectionType connectionType) : base("Kubernetes@1")
    {
        Command = command;
        ConnectionType = connectionType;
    }
}

/// <summary>A Kubernetes@1 task using a Kubernetes service connection.</summary>
public record KubernetesServiceConnectionTask : KubernetesV1Task
{
    /// <summary>Kubernetes service connection.</summary>
    [YamlIgnore] public AdoExpression<string>? KubernetesServiceEndpoint { get => GetExpression<string>("kubernetesServiceEndpoint"); init => SetProperty("kubernetesServiceEndpoint", value); }

    /// <summary>Initializes a Kubernetes service connection task.</summary>
    public KubernetesServiceConnectionTask(KubernetesCommand command, AdoExpression<string> kubernetesServiceEndpoint)
        : base(command, KubernetesConnectionType.KubernetesServiceConnection) => KubernetesServiceEndpoint = kubernetesServiceEndpoint;
}

/// <summary>A Kubernetes@1 task using an Azure Resource Manager connection to an AKS cluster.</summary>
public record AzureResourceManagerKubernetesTask : KubernetesV1Task
{
    /// <summary>Azure Resource Manager service connection.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureSubscriptionEndpoint { get => GetExpression<string>("azureSubscriptionEndpoint"); init => SetProperty("azureSubscriptionEndpoint", value); }
    /// <summary>Resource group containing the AKS cluster.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureResourceGroup { get => GetExpression<string>("azureResourceGroup"); init => SetProperty("azureResourceGroup", value); }
    /// <summary>AKS cluster name.</summary>
    [YamlIgnore] public AdoExpression<string>? KubernetesCluster { get => GetExpression<string>("kubernetesCluster"); init => SetProperty("kubernetesCluster", value); }
    /// <summary>Whether to use cluster administrator credentials.</summary>
    [YamlIgnore] public AdoExpression<bool>? UseClusterAdmin { get => GetExpression<bool>("useClusterAdmin"); init => SetProperty("useClusterAdmin", value); }

    /// <summary>Initializes an Azure Resource Manager Kubernetes task.</summary>
    public AzureResourceManagerKubernetesTask(KubernetesCommand command, AdoExpression<string> azureSubscriptionEndpoint, AdoExpression<string> azureResourceGroup, AdoExpression<string> kubernetesCluster)
        : base(command, KubernetesConnectionType.AzureResourceManager)
    {
        AzureSubscriptionEndpoint = azureSubscriptionEndpoint;
        AzureResourceGroup = azureResourceGroup;
        KubernetesCluster = kubernetesCluster;
    }
}

/// <summary>A Kubernetes@1 task that uses the agent's existing Kubernetes configuration.</summary>
public record KubernetesNoConnectionTask(KubernetesCommand command) : KubernetesV1Task(command, KubernetesConnectionType.None);

/// <summary>Models the deprecated <c>Kubernetes@0</c> task for existing pipelines. See the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/KubernetesV0/task.json">task specification</see>.</summary>
public record KubernetesV0Task(KubernetesCommand command, AdoExpression<string> kubernetesServiceEndpoint) : KubernetesTask("Kubernetes@0")
{
    /// <summary>Kubernetes service connection.</summary>
    [YamlIgnore] public AdoExpression<string>? KubernetesServiceEndpoint { get => GetExpression<string>("kubernetesServiceEndpoint"); init => SetProperty("kubernetesServiceEndpoint", value); } = kubernetesServiceEndpoint;
    /// <summary>kubectl command to run.</summary>
    [YamlIgnore] public AdoExpression<KubernetesCommand>? Command { get => GetExpression<KubernetesCommand>("command"); init => SetProperty("command", value); } = command;
    /// <summary>Name of the variable receiving the kubectl output.</summary>
    [YamlIgnore] public AdoExpression<string>? KubectlOutput { get => GetExpression<string>("kubectlOutput"); init => SetProperty("kubectlOutput", value); }
}

/// <summary>Connection types supported by Kubernetes@1.</summary>
public enum KubernetesConnectionType
{
    /// <summary>Azure Resource Manager connection.</summary>
    [YamlMember(Alias = "Azure Resource Manager")] AzureResourceManager,
    /// <summary>Kubernetes service connection.</summary>
    [YamlMember(Alias = "Kubernetes Service Connection")] KubernetesServiceConnection,
    /// <summary>Use the agent's existing configuration.</summary>
    [YamlMember(Alias = "None")] None,
}

/// <summary>Commands supported by Kubernetes@1.</summary>
public enum KubernetesCommand
{
    [YamlMember(Alias = "apply")] Apply, [YamlMember(Alias = "create")] Create, [YamlMember(Alias = "delete")] Delete,
    [YamlMember(Alias = "exec")] Exec, [YamlMember(Alias = "expose")] Expose, [YamlMember(Alias = "get")] Get,
    [YamlMember(Alias = "login")] Login, [YamlMember(Alias = "logout")] Logout, [YamlMember(Alias = "logs")] Logs,
    [YamlMember(Alias = "rollout")] Rollout, [YamlMember(Alias = "run")] Run, [YamlMember(Alias = "set")] Set,
    [YamlMember(Alias = "top")] Top,
}

/// <summary>Configuration source types supported by Kubernetes@1.</summary>
public enum KubernetesConfigurationType { [YamlMember(Alias = "configuration")] Configuration, [YamlMember(Alias = "inline")] Inline }
/// <summary>Secret types supported by Kubernetes@1.</summary>
public enum KubernetesSecretType { [YamlMember(Alias = "dockerRegistry")] DockerRegistry, [YamlMember(Alias = "generic")] Generic }
/// <summary>Container registry types supported by Kubernetes@1.</summary>
public enum KubernetesContainerRegistryType { [YamlMember(Alias = "Azure Container Registry")] AzureContainerRegistry, [YamlMember(Alias = "Container Registry")] ContainerRegistry }
/// <summary>kubectl selection types supported by Kubernetes@1.</summary>
public enum KubectlLocationType { [YamlMember(Alias = "version")] Version, [YamlMember(Alias = "location")] Location }
/// <summary>kubectl output formats supported by Kubernetes@1.</summary>
public enum KubernetesOutputFormat { [YamlMember(Alias = "json")] Json, [YamlMember(Alias = "yaml")] Yaml, [YamlMember(Alias = "none")] None }
