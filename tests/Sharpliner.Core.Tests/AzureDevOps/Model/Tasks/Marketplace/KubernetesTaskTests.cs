using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class KubernetesTaskTests
{
    [Fact]
    public Task Serialize_ServiceConnection_Task_Test()
    {
        var task = new KubernetesServiceConnectionTask(KubernetesCommand.Apply, "kubernetes-connection")
        {
            Namespace = "production",
            UseConfigurationFile = true,
            ConfigurationType = KubernetesConfigurationType.Inline,
            InlineConfiguration = "apiVersion: v1",
            Arguments = "--validate=true",
            OutputFormat = KubernetesOutputFormat.Yaml,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AzureResourceManager_Task_With_Secret_Test()
    {
        var task = new AzureResourceManagerKubernetesTask(KubernetesCommand.Create, "azure-connection", "production-rg", "production-aks")
        {
            UseClusterAdmin = true,
            SecretType = KubernetesSecretType.DockerRegistry,
            ContainerRegistryType = KubernetesContainerRegistryType.AzureContainerRegistry,
            AzureSubscriptionEndpointForSecrets = "registry-connection",
            AzureContainerRegistry = "registry.azurecr.io",
            SecretName = "registry-secret",
            ForceUpdate = true,
            VersionOrLocation = KubectlLocationType.Location,
            SpecifyLocation = "/usr/local/bin/kubectl",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_NoConnection_Task_With_ConfigMap_Test()
    {
        var task = new KubernetesNoConnectionTask(KubernetesCommand.Create)
        {
            ConfigMapName = "application-settings",
            ForceUpdateConfigMap = true,
            UseConfigMapFile = false,
            ConfigMapArguments = "--from-literal=setting=value",
            VersionSpec = "1.30.0",
            CheckLatest = false,
            WorkingDirectory = "manifests",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V0_Task_Test()
    {
        var task = new KubernetesV0Task(KubernetesCommand.Get, "legacy-kubernetes-connection")
        {
            KubectlOutput = "kubectlOutput",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    private sealed class KubernetesPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";
        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("deploy")
                {
                    Steps =
                    {
                        Kubernetes.ServiceConnection(KubernetesCommand.Rollout, "kubernetes-connection", "Roll out application"),
                        Kubernetes.AzureResourceManager(KubernetesCommand.Get, "azure-connection", "production-rg", "production-aks"),
                        Kubernetes.None(KubernetesCommand.Logout),
                    },
                },
            },
        };
    }

    [Fact]
    public Task Serialize_Kubernetes_Builder_Test() => Verify(new KubernetesPipeline().Serialize());
}
