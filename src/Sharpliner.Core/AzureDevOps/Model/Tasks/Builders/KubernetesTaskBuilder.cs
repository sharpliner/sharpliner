using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>Creates strongly typed <c>Kubernetes@1</c> Kubectl tasks. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/kubernetes-v1?view=azure-pipelines">official reference</see>.</summary>
public class KubernetesTaskBuilder : TaskBuilderBase
{
    /// <summary>Creates a task using a Kubernetes service connection.</summary>
    public KubernetesServiceConnectionTask ServiceConnection(KubernetesCommand command, AdoExpression<string> kubernetesServiceEndpoint, AdoExpression<string>? displayName = null) =>
        new(command, kubernetesServiceEndpoint) { DisplayName = displayName! };

    /// <summary>Creates a task using an Azure Resource Manager connection to an AKS cluster.</summary>
    public AzureResourceManagerKubernetesTask AzureResourceManager(KubernetesCommand command, AdoExpression<string> azureSubscriptionEndpoint, AdoExpression<string> azureResourceGroup, AdoExpression<string> kubernetesCluster, AdoExpression<string>? displayName = null) =>
        new(command, azureSubscriptionEndpoint, azureResourceGroup, kubernetesCluster) { DisplayName = displayName! };

    /// <summary>Creates a task using the Kubernetes configuration already available on the agent.</summary>
    public KubernetesNoConnectionTask None(KubernetesCommand command, AdoExpression<string>? displayName = null) =>
        new(command) { DisplayName = displayName! };

    internal KubernetesTaskBuilder() { }
}
