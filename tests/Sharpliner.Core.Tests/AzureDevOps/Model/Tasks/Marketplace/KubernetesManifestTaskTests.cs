using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class KubernetesManifestTaskTests
{
    [Fact]
    public Task Serialize_Deploy_Task_Test()
    {
        var task = new KubernetesManifestDeployV1Task
        {
            ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
            KubernetesServiceEndpoint = "aks-connection",
            Namespace = "production",
            Strategy = KubernetesManifestStrategy.Canary,
            TrafficSplitMethod = KubernetesManifestTrafficSplitMethod.Smi,
            Percentage = "20",
            BaselineAndCanaryReplicas = "1",
            Manifests = "k8s/deployment.yml\nk8s/service.yml",
            Containers = "sample/app:2.0.0",
            ImagePullSecrets = "acr-secret",
            RolloutStatusTimeout = "300",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Bake_Task_Helm_Test()
    {
        var task = new KubernetesManifestBakeV1Task
        {
            Namespace = "staging",
            RenderType = KubernetesManifestRenderType.Helm,
            HelmChart = "charts/web",
            ReleaseName = "webapp",
            OverrideFiles = "values/common.yml\nvalues/staging.yml",
            Overrides = "image.tag=2.0.0",
            Containers = "sample/web:2.0.0",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Patch_Task_Test()
    {
        var task = new KubernetesManifestPatchV1Task
        {
            ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
            KubernetesServiceEndpoint = "aks-connection",
            Namespace = "production",
            ResourceToPatch = KubernetesManifestResourceToPatch.Name,
            Kind = KubernetesManifestKind.Deployment,
            ResourceName = "webapp",
            MergeStrategy = KubernetesManifestMergeStrategy.Strategic,
            Patch = "{\"spec\":{\"replicas\":5}}",
            RolloutStatusTimeout = "120",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_CreateSecret_Task_Generic_Test()
    {
        var task = new KubernetesManifestCreateSecretV1Task
        {
            ConnectionType = KubernetesManifestConnectionType.KubernetesServiceConnection,
            KubernetesServiceEndpoint = "aks-connection",
            Namespace = "production",
            SecretType = KubernetesManifestSecretType.Generic,
            SecretName = "app-secrets",
            SecretArguments = "--from-literal=ConnectionString=Server=tcp",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Delete_Task_AzureRm_Test()
    {
        var task = new KubernetesManifestDeleteV1Task
        {
            ConnectionType = KubernetesManifestConnectionType.AzureResourceManager,
            AzureSubscriptionEndpoint = "azure-rm-connection",
            AzureResourceGroup = "rg-sharpliner",
            KubernetesCluster = "aks-sharpliner",
            Namespace = "production",
            Arguments = "-f k8s/obsolete.yml",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
