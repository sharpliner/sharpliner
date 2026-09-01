using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents common inputs for <c>KubernetesManifest@1</c> actions.
/// </summary>
/// <remarks>
/// See the official task reference at
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/kubernetes-manifest-v1?view=azure-pipelines" />.
/// </remarks>
public abstract record KubernetesManifestV1Task : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestV1Task"/> class.
    /// </summary>
    /// <param name="action">Task action value.</param>
    protected KubernetesManifestV1Task(string action) : base("KubernetesManifest@1")
    {
        Action = action;
    }

    [YamlIgnore]
    internal AdoExpression<string>? Action
    {
        get => GetExpression<string>("action");
        init => SetProperty("action", value);
    }

    /// <summary>
    /// Kubernetes namespace where resources are managed.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Namespace
    {
        get => GetExpression<string>("namespace");
        init => SetProperty("namespace", value);
    }
}

/// <summary>
/// Represents common cluster connection inputs for <c>KubernetesManifest@1</c> actions that connect to a cluster.
/// </summary>
public abstract record KubernetesManifestConnectedV1Task : KubernetesManifestV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestConnectedV1Task"/> class.
    /// </summary>
    /// <param name="action">Task action value.</param>
    protected KubernetesManifestConnectedV1Task(string action) : base(action)
    {
    }

    /// <summary>
    /// Connection mode. Use <see cref="KubernetesManifestConnectionType.KubernetesServiceConnection"/> or
    /// <see cref="KubernetesManifestConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestConnectionType>? ConnectionType
    {
        get => GetExpression<KubernetesManifestConnectionType>("connectionType");
        init => SetProperty("connectionType", value);
    }

    /// <summary>
    /// Kubernetes service connection name. Used when <see cref="ConnectionType"/> is
    /// <see cref="KubernetesManifestConnectionType.KubernetesServiceConnection"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KubernetesServiceEndpoint
    {
        get => GetExpression<string>("kubernetesServiceEndpoint");
        init => SetProperty("kubernetesServiceEndpoint", value);
    }

    /// <summary>
    /// Azure Resource Manager service connection. Used when <see cref="ConnectionType"/> is
    /// <see cref="KubernetesManifestConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscriptionEndpoint
    {
        get => GetExpression<string>("azureSubscriptionEndpoint");
        init => SetProperty("azureSubscriptionEndpoint", value);
    }

    /// <summary>
    /// Azure resource group that contains the AKS cluster when using
    /// <see cref="KubernetesManifestConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureResourceGroup
    {
        get => GetExpression<string>("azureResourceGroup");
        init => SetProperty("azureResourceGroup", value);
    }

    /// <summary>
    /// AKS cluster name when using <see cref="KubernetesManifestConnectionType.AzureResourceManager"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KubernetesCluster
    {
        get => GetExpression<string>("kubernetesCluster");
        init => SetProperty("kubernetesCluster", value);
    }

    /// <summary>
    /// When true, uses cluster-admin credentials for Azure Resource Manager connections.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseClusterAdmin
    {
        get => GetExpression<bool>("useClusterAdmin");
        init => SetProperty("useClusterAdmin", value);
    }
}

/// <summary>
/// Deploy action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestDeployV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestDeployV1Task"/> class.
    /// </summary>
    public KubernetesManifestDeployV1Task() : base("deploy")
    {
    }

    /// <summary>
    /// Deployment strategy. Defaults to <see cref="KubernetesManifestStrategy.None"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestStrategy>? Strategy
    {
        get => GetExpression<KubernetesManifestStrategy>("strategy");
        init => SetProperty("strategy", value);
    }

    /// <summary>
    /// Canary traffic split method when <see cref="Strategy"/> is canary.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestTrafficSplitMethod>? TrafficSplitMethod
    {
        get => GetExpression<KubernetesManifestTrafficSplitMethod>("trafficSplitMethod");
        init => SetProperty("trafficSplitMethod", value);
    }

    /// <summary>
    /// Canary traffic percentage for deploy action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Percentage
    {
        get => GetExpression<string>("percentage");
        init => SetProperty("percentage", value);
    }

    /// <summary>
    /// Baseline and canary replicas used with SMI traffic splitting during canary deploy.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BaselineAndCanaryReplicas
    {
        get => GetExpression<string>("baselineAndCanaryReplicas");
        init => SetProperty("baselineAndCanaryReplicas", value);
    }

    /// <summary>
    /// Manifest file paths (newline-separated patterns).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Manifests
    {
        get => GetExpression<string>("manifests");
        init => SetProperty("manifests", value);
    }

    /// <summary>
    /// Container image substitutions (newline-separated <c>image:tag</c> values).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Containers
    {
        get => GetExpression<string>("containers");
        init => SetProperty("containers", value);
    }

    /// <summary>
    /// Image pull secrets (newline-separated secret names).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ImagePullSecrets
    {
        get => GetExpression<string>("imagePullSecrets");
        init => SetProperty("imagePullSecrets", value);
    }

    /// <summary>
    /// Rollout status timeout in seconds. <c>0</c> means no timeout.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RolloutStatusTimeout
    {
        get => GetExpression<string>("rolloutStatusTimeout");
        init => SetProperty("rolloutStatusTimeout", value);
    }

    /// <summary>
    /// ARM resource type used to discover Kubernetes resources. Default is
    /// <c>Microsoft.ContainerService/managedClusters</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceType
    {
        get => GetExpression<string>("resourceType");
        init => SetProperty("resourceType", value);
    }
}

/// <summary>
/// Promote action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestPromoteV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestPromoteV1Task"/> class.
    /// </summary>
    public KubernetesManifestPromoteV1Task() : base("promote")
    {
    }

    /// <summary>
    /// Promotion strategy. Set to canary to control traffic split behavior.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestStrategy>? Strategy
    {
        get => GetExpression<KubernetesManifestStrategy>("strategy");
        init => SetProperty("strategy", value);
    }

    /// <summary>
    /// Canary traffic split method used when <see cref="Strategy"/> is canary.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestTrafficSplitMethod>? TrafficSplitMethod
    {
        get => GetExpression<KubernetesManifestTrafficSplitMethod>("trafficSplitMethod");
        init => SetProperty("trafficSplitMethod", value);
    }

    /// <summary>
    /// Manifest file paths (newline-separated patterns) used by the promote action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Manifests
    {
        get => GetExpression<string>("manifests");
        init => SetProperty("manifests", value);
    }

    /// <summary>
    /// Container image substitutions (newline-separated <c>image:tag</c> values).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Containers
    {
        get => GetExpression<string>("containers");
        init => SetProperty("containers", value);
    }

    /// <summary>
    /// Image pull secrets (newline-separated secret names).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ImagePullSecrets
    {
        get => GetExpression<string>("imagePullSecrets");
        init => SetProperty("imagePullSecrets", value);
    }

    /// <summary>
    /// Rollout status timeout in seconds for the promote action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RolloutStatusTimeout
    {
        get => GetExpression<string>("rolloutStatusTimeout");
        init => SetProperty("rolloutStatusTimeout", value);
    }
}

/// <summary>
/// Reject action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestRejectV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestRejectV1Task"/> class.
    /// </summary>
    public KubernetesManifestRejectV1Task() : base("reject")
    {
    }

    /// <summary>
    /// Reject strategy. Set to canary to reject canary traffic state.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestStrategy>? Strategy
    {
        get => GetExpression<KubernetesManifestStrategy>("strategy");
        init => SetProperty("strategy", value);
    }

    /// <summary>
    /// Canary traffic split method used when <see cref="Strategy"/> is canary.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestTrafficSplitMethod>? TrafficSplitMethod
    {
        get => GetExpression<KubernetesManifestTrafficSplitMethod>("trafficSplitMethod");
        init => SetProperty("trafficSplitMethod", value);
    }

    /// <summary>
    /// Manifest file paths (newline-separated patterns) used by the reject action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Manifests
    {
        get => GetExpression<string>("manifests");
        init => SetProperty("manifests", value);
    }
}

/// <summary>
/// Bake action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestBakeV1Task : KubernetesManifestV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestBakeV1Task"/> class.
    /// </summary>
    public KubernetesManifestBakeV1Task() : base("bake")
    {
    }

    /// <summary>
    /// Bake renderer type (<c>helm</c>, <c>kompose</c>, or <c>kustomize</c>).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestRenderType>? RenderType
    {
        get => GetExpression<KubernetesManifestRenderType>("renderType");
        init => SetProperty("renderType", value);
    }

    /// <summary>
    /// Docker Compose file path when <see cref="RenderType"/> is kompose.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerComposeFile
    {
        get => GetExpression<string>("dockerComposeFile");
        init => SetProperty("dockerComposeFile", value);
    }

    /// <summary>
    /// Helm chart path when <see cref="RenderType"/> is helm.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? HelmChart
    {
        get => GetExpression<string>("helmChart");
        init => SetProperty("helmChart", value);
    }

    /// <summary>
    /// Helm release name used during bake.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ReleaseName
    {
        get => GetExpression<string>("releaseName");
        init => SetProperty("releaseName", value);
    }

    /// <summary>
    /// Helm values files (newline-separated paths) used during bake.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideFiles
    {
        get => GetExpression<string>("overrideFiles");
        init => SetProperty("overrideFiles", value);
    }

    /// <summary>
    /// Helm overrides values in key-value form.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Overrides
    {
        get => GetExpression<string>("overrides");
        init => SetProperty("overrides", value);
    }

    /// <summary>
    /// Kustomization path when <see cref="RenderType"/> is kustomize.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KustomizationPath
    {
        get => GetExpression<string>("kustomizationPath");
        init => SetProperty("kustomizationPath", value);
    }

    /// <summary>
    /// Container image substitutions (newline-separated <c>image:tag</c> values).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Containers
    {
        get => GetExpression<string>("containers");
        init => SetProperty("containers", value);
    }
}

/// <summary>
/// Scale action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestScaleV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestScaleV1Task"/> class.
    /// </summary>
    public KubernetesManifestScaleV1Task() : base("scale")
    {
    }

    /// <summary>
    /// Resource kind to scale.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestKind>? Kind
    {
        get => GetExpression<KubernetesManifestKind>("kind");
        init => SetProperty("kind", value);
    }

    /// <summary>
    /// Resource name to scale. Emits the <c>name</c> task input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceName
    {
        get => GetExpression<string>("name");
        init => SetProperty("name", value);
    }

    /// <summary>
    /// Target replica count.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Replicas
    {
        get => GetExpression<string>("replicas");
        init => SetProperty("replicas", value);
    }

    /// <summary>
    /// Rollout status timeout in seconds for the scale action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RolloutStatusTimeout
    {
        get => GetExpression<string>("rolloutStatusTimeout");
        init => SetProperty("rolloutStatusTimeout", value);
    }
}

/// <summary>
/// Patch action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestPatchV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestPatchV1Task"/> class.
    /// </summary>
    public KubernetesManifestPatchV1Task() : base("patch")
    {
    }

    /// <summary>
    /// Patch target mode: patch from file or patch by named resource.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestResourceToPatch>? ResourceToPatch
    {
        get => GetExpression<KubernetesManifestResourceToPatch>("resourceToPatch");
        init => SetProperty("resourceToPatch", value);
    }

    /// <summary>
    /// Manifest file path to patch when <see cref="ResourceToPatch"/> is file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceFileToPatch
    {
        get => GetExpression<string>("resourceFileToPatch");
        init => SetProperty("resourceFileToPatch", value);
    }

    /// <summary>
    /// Resource kind to patch when <see cref="ResourceToPatch"/> is name.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestKind>? Kind
    {
        get => GetExpression<KubernetesManifestKind>("kind");
        init => SetProperty("kind", value);
    }

    /// <summary>
    /// Resource name to patch when <see cref="ResourceToPatch"/> is name. Emits the <c>name</c> task input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceName
    {
        get => GetExpression<string>("name");
        init => SetProperty("name", value);
    }

    /// <summary>
    /// Patch merge strategy.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestMergeStrategy>? MergeStrategy
    {
        get => GetExpression<KubernetesManifestMergeStrategy>("mergeStrategy");
        init => SetProperty("mergeStrategy", value);
    }

    /// <summary>
    /// Patch payload content.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Patch
    {
        get => GetExpression<string>("patch");
        init => SetProperty("patch", value);
    }

    /// <summary>
    /// Rollout status timeout in seconds for the patch action.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RolloutStatusTimeout
    {
        get => GetExpression<string>("rolloutStatusTimeout");
        init => SetProperty("rolloutStatusTimeout", value);
    }
}

/// <summary>
/// Delete action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestDeleteV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestDeleteV1Task"/> class.
    /// </summary>
    public KubernetesManifestDeleteV1Task() : base("delete")
    {
    }

    /// <summary>
    /// Additional arguments passed to the delete command.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("arguments");
        init => SetProperty("arguments", value);
    }
}

/// <summary>
/// Create-secret action for <c>KubernetesManifest@1</c>.
/// </summary>
public record KubernetesManifestCreateSecretV1Task : KubernetesManifestConnectedV1Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KubernetesManifestCreateSecretV1Task"/> class.
    /// </summary>
    public KubernetesManifestCreateSecretV1Task() : base("createSecret")
    {
    }

    /// <summary>
    /// Secret creation mode.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<KubernetesManifestSecretType>? SecretType
    {
        get => GetExpression<KubernetesManifestSecretType>("secretType");
        init => SetProperty("secretType", value);
    }

    /// <summary>
    /// Secret name to create in the target namespace.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SecretName
    {
        get => GetExpression<string>("secretName");
        init => SetProperty("secretName", value);
    }

    /// <summary>
    /// Arguments for generic secret creation when <see cref="SecretType"/> is generic.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SecretArguments
    {
        get => GetExpression<string>("secretArguments");
        init => SetProperty("secretArguments", value);
    }

    /// <summary>
    /// Docker registry service connection when <see cref="SecretType"/> is dockerRegistry.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerRegistryEndpoint
    {
        get => GetExpression<string>("dockerRegistryEndpoint");
        init => SetProperty("dockerRegistryEndpoint", value);
    }
}

/// <summary>
/// Connection type options for <c>KubernetesManifest@1</c>.
/// </summary>
public enum KubernetesManifestConnectionType
{
    /// <summary>
    /// Azure Resource Manager connection to AKS.
    /// </summary>
    [YamlMember(Alias = "azureResourceManager")]
    AzureResourceManager,

    /// <summary>
    /// Kubernetes service connection.
    /// </summary>
    [YamlMember(Alias = "kubernetesServiceConnection")]
    KubernetesServiceConnection,
}

/// <summary>
/// Deployment strategy options.
/// </summary>
public enum KubernetesManifestStrategy
{
    /// <summary>
    /// No canary strategy.
    /// </summary>
    [YamlMember(Alias = "none")]
    None,

    /// <summary>
    /// Canary strategy.
    /// </summary>
    [YamlMember(Alias = "canary")]
    Canary,
}

/// <summary>
/// Canary traffic split method.
/// </summary>
public enum KubernetesManifestTrafficSplitMethod
{
    /// <summary>
    /// Pod based split.
    /// </summary>
    [YamlMember(Alias = "pod")]
    Pod,

    /// <summary>
    /// SMI based split.
    /// </summary>
    [YamlMember(Alias = "smi")]
    Smi,
}

/// <summary>
/// Bake render type options.
/// </summary>
public enum KubernetesManifestRenderType
{
    /// <summary>
    /// Render with Helm.
    /// </summary>
    [YamlMember(Alias = "helm")]
    Helm,

    /// <summary>
    /// Render with Kompose.
    /// </summary>
    [YamlMember(Alias = "kompose")]
    Kompose,

    /// <summary>
    /// Render with Kustomize.
    /// </summary>
    [YamlMember(Alias = "kustomize")]
    Kustomize,
}

/// <summary>
/// Patch target mode options.
/// </summary>
public enum KubernetesManifestResourceToPatch
{
    /// <summary>
    /// Patch from a file.
    /// </summary>
    [YamlMember(Alias = "file")]
    File,

    /// <summary>
    /// Patch by resource name.
    /// </summary>
    [YamlMember(Alias = "name")]
    Name,
}

/// <summary>
/// Kubernetes resource kind options used by patch/scale actions.
/// </summary>
public enum KubernetesManifestKind
{
    /// <summary>
    /// Deployment resource.
    /// </summary>
    [YamlMember(Alias = "deployment")]
    Deployment,

    /// <summary>
    /// ReplicaSet resource.
    /// </summary>
    [YamlMember(Alias = "replicaset")]
    Replicaset,

    /// <summary>
    /// StatefulSet resource.
    /// </summary>
    [YamlMember(Alias = "statefulset")]
    Statefulset,
}

/// <summary>
/// Patch merge strategy options.
/// </summary>
public enum KubernetesManifestMergeStrategy
{
    /// <summary>
    /// JSON patch strategy.
    /// </summary>
    [YamlMember(Alias = "json")]
    Json,

    /// <summary>
    /// Merge patch strategy.
    /// </summary>
    [YamlMember(Alias = "merge")]
    Merge,

    /// <summary>
    /// Strategic merge patch strategy.
    /// </summary>
    [YamlMember(Alias = "strategic")]
    Strategic,
}

/// <summary>
/// Secret type options for create-secret action.
/// </summary>
public enum KubernetesManifestSecretType
{
    /// <summary>
    /// Docker registry secret mode.
    /// </summary>
    [YamlMember(Alias = "dockerRegistry")]
    DockerRegistry,

    /// <summary>
    /// Generic secret mode.
    /// </summary>
    [YamlMember(Alias = "generic")]
    Generic,
}
