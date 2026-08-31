using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for strongly typed <c>KubernetesManifest@1</c> task actions.
/// </summary>
/// <remarks>
/// Official task reference:
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/kubernetes-manifest-v1?view=azure-pipelines" />.
/// </remarks>
public class KubernetesManifestTaskBuilder
{
    /// <summary>
    /// Gets a deploy action builder.
    /// </summary>
    public KubernetesManifestDeployBuilder Deploy => new();

    /// <summary>
    /// Gets a promote action builder.
    /// </summary>
    public KubernetesManifestPromoteBuilder Promote => new();

    /// <summary>
    /// Gets a reject action builder.
    /// </summary>
    public KubernetesManifestRejectBuilder Reject => new();

    /// <summary>
    /// Gets a bake action builder.
    /// </summary>
    public KubernetesManifestBakeBuilder Bake => new();

    /// <summary>
    /// Gets a patch action builder.
    /// </summary>
    public KubernetesManifestPatchBuilder Patch => new();

    /// <summary>
    /// Gets a scale action builder.
    /// </summary>
    public KubernetesManifestScaleBuilder Scale => new();

    /// <summary>
    /// Gets a delete action builder.
    /// </summary>
    public KubernetesManifestDeleteBuilder Delete => new();

    /// <summary>
    /// Gets a create-secret action builder.
    /// </summary>
    public KubernetesManifestCreateSecretBuilder CreateSecret => new();
}

/// <summary>
/// Builder for deploy action.
/// </summary>
public class KubernetesManifestDeployBuilder
{
    /// <summary>
    /// Creates deploy action with a Kubernetes service connection.
    /// </summary>
    /// <param name="kubernetesServiceEndpoint">Kubernetes service connection name.</param>
    /// <param name="manifests">Manifest paths (newline-separated patterns).</param>
    public KubernetesManifestDeployV1Task WithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint, AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        Manifests = manifests,
    };

    /// <summary>
    /// Creates deploy action with an Azure Resource Manager connection.
    /// </summary>
    public KubernetesManifestDeployV1Task WithAzureResourceManager(
        AdoExpression<string> azureSubscriptionEndpoint,
        AdoExpression<string> azureResourceGroup,
        AdoExpression<string> kubernetesCluster,
        AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.AzureResourceManager,
        AzureSubscriptionEndpoint = azureSubscriptionEndpoint,
        AzureResourceGroup = azureResourceGroup,
        KubernetesCluster = kubernetesCluster,
        Manifests = manifests,
    };
}

/// <summary>
/// Builder for promote action.
/// </summary>
public class KubernetesManifestPromoteBuilder
{
    /// <summary>
    /// Creates promote action with a Kubernetes service connection.
    /// </summary>
    public KubernetesManifestPromoteV1Task WithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint, AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        Manifests = manifests,
    };

    /// <summary>
    /// Creates promote action with an Azure Resource Manager connection.
    /// </summary>
    public KubernetesManifestPromoteV1Task WithAzureResourceManager(
        AdoExpression<string> azureSubscriptionEndpoint,
        AdoExpression<string> azureResourceGroup,
        AdoExpression<string> kubernetesCluster,
        AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.AzureResourceManager,
        AzureSubscriptionEndpoint = azureSubscriptionEndpoint,
        AzureResourceGroup = azureResourceGroup,
        KubernetesCluster = kubernetesCluster,
        Manifests = manifests,
    };
}

/// <summary>
/// Builder for reject action.
/// </summary>
public class KubernetesManifestRejectBuilder
{
    /// <summary>
    /// Creates reject action with a Kubernetes service connection.
    /// </summary>
    public KubernetesManifestRejectV1Task WithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint, AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        Manifests = manifests,
    };

    /// <summary>
    /// Creates reject action with an Azure Resource Manager connection.
    /// </summary>
    public KubernetesManifestRejectV1Task WithAzureResourceManager(
        AdoExpression<string> azureSubscriptionEndpoint,
        AdoExpression<string> azureResourceGroup,
        AdoExpression<string> kubernetesCluster,
        AdoExpression<string> manifests) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.AzureResourceManager,
        AzureSubscriptionEndpoint = azureSubscriptionEndpoint,
        AzureResourceGroup = azureResourceGroup,
        KubernetesCluster = kubernetesCluster,
        Manifests = manifests,
    };
}

/// <summary>
/// Builder for bake action.
/// </summary>
public class KubernetesManifestBakeBuilder
{
    /// <summary>
    /// Creates a Helm bake action.
    /// </summary>
    public KubernetesManifestBakeV1Task Helm(AdoExpression<string> helmChart) => new()
    {
        RenderType = KubernetesManifestRenderType.Helm,
        HelmChart = helmChart,
    };

    /// <summary>
    /// Creates a Kompose bake action.
    /// </summary>
    public KubernetesManifestBakeV1Task Kompose(AdoExpression<string> dockerComposeFile) => new()
    {
        RenderType = KubernetesManifestRenderType.Kompose,
        DockerComposeFile = dockerComposeFile,
    };

    /// <summary>
    /// Creates a Kustomize bake action.
    /// </summary>
    public KubernetesManifestBakeV1Task Kustomize(AdoExpression<string> kustomizationPath) => new()
    {
        RenderType = KubernetesManifestRenderType.Kustomize,
        KustomizationPath = kustomizationPath,
    };
}

/// <summary>
/// Builder for patch action.
/// </summary>
public class KubernetesManifestPatchBuilder
{
    /// <summary>
    /// Creates patch action that patches resources from a manifest file.
    /// </summary>
    public KubernetesManifestPatchV1Task FileWithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint, AdoExpression<string> resourceFileToPatch, AdoExpression<string> patch) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        ResourceToPatch = KubernetesManifestResourceToPatch.File,
        ResourceFileToPatch = resourceFileToPatch,
        Patch = patch,
    };

    /// <summary>
    /// Creates patch action that patches a named resource.
    /// </summary>
    public KubernetesManifestPatchV1Task NamedWithKubernetesServiceConnection(
        AdoExpression<string> kubernetesServiceEndpoint,
        KubernetesManifestKind kind,
        AdoExpression<string> name,
        AdoExpression<string> patch) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        ResourceToPatch = KubernetesManifestResourceToPatch.Name,
        Kind = kind,
        ResourceName = name,
        Patch = patch,
    };
}

/// <summary>
/// Builder for scale action.
/// </summary>
public class KubernetesManifestScaleBuilder
{
    /// <summary>
    /// Creates scale action with a Kubernetes service connection.
    /// </summary>
    public KubernetesManifestScaleV1Task WithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint, KubernetesManifestKind kind, AdoExpression<string> name, AdoExpression<string> replicas) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        Kind = kind,
        ResourceName = name,
        Replicas = replicas,
    };
}

/// <summary>
/// Builder for delete action.
/// </summary>
public class KubernetesManifestDeleteBuilder
{
    /// <summary>
    /// Creates delete action with a Kubernetes service connection.
    /// </summary>
    public KubernetesManifestDeleteV1Task WithKubernetesServiceConnection(AdoExpression<string> kubernetesServiceEndpoint) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
    };

    /// <summary>
    /// Creates delete action with an Azure Resource Manager connection.
    /// </summary>
    public KubernetesManifestDeleteV1Task WithAzureResourceManager(
        AdoExpression<string> azureSubscriptionEndpoint,
        AdoExpression<string> azureResourceGroup,
        AdoExpression<string> kubernetesCluster) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.AzureResourceManager,
        AzureSubscriptionEndpoint = azureSubscriptionEndpoint,
        AzureResourceGroup = azureResourceGroup,
        KubernetesCluster = kubernetesCluster,
    };
}

/// <summary>
/// Builder for create-secret action.
/// </summary>
public class KubernetesManifestCreateSecretBuilder
{
    /// <summary>
    /// Creates Docker registry secret mode.
    /// </summary>
    public KubernetesManifestCreateSecretV1Task DockerRegistryWithKubernetesServiceConnection(
        AdoExpression<string> kubernetesServiceEndpoint,
        AdoExpression<string> secretName,
        AdoExpression<string> dockerRegistryEndpoint) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        SecretType = KubernetesManifestSecretType.DockerRegistry,
        SecretName = secretName,
        DockerRegistryEndpoint = dockerRegistryEndpoint,
    };

    /// <summary>
    /// Creates generic secret mode.
    /// </summary>
    public KubernetesManifestCreateSecretV1Task GenericWithKubernetesServiceConnection(
        AdoExpression<string> kubernetesServiceEndpoint,
        AdoExpression<string> secretName,
        AdoExpression<string> secretArguments) => new()
    {
        ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
        KubernetesServiceEndpoint = kubernetesServiceEndpoint,
        SecretType = KubernetesManifestSecretType.Generic,
        SecretName = secretName,
        SecretArguments = secretArguments,
    };
}
