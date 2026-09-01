using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class HelmDeployTaskTests
{
    private class HelmPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            [
                new Job("job")
                {
                    Steps =
                    [
                        Helm.Install.FromChartName("stable/mysql") with
                        {
                            ConnectionType = HelmConnectionType.AzureResourceManager,
                            AzureSubscriptionEndpoint = "my-subscription",
                            AzureResourceGroup = "my-resource-group",
                            KubernetesCluster = "my-cluster",
                            Namespace = "my-namespace",
                            ReleaseName = "my-release",
                            OverrideValues = "key1=val1,key2=val2",
                            ValueFile = "values.yaml",
                            UpdateDependency = true,
                            WaitForExecution = false,
                        },
                        Helm.Upgrade.FromChartPath("./charts/redis") with
                        {
                            ConnectionType = HelmConnectionType.KubernetesServiceConnection,
                            KubernetesServiceEndpoint = "my-k8s-connection",
                            ReleaseName = "my-release",
                            Version = "1.2.3",
                            Install = true,
                            Recreate = true,
                            ResetValues = true,
                            Force = true,
                            Arguments = "--dry-run",
                        },
                        Helm.Package("./charts/redis") with
                        {
                            Destination = "$(Build.ArtifactStagingDirectory)",
                            Save = false,
                            UpdateDependency = true,
                            Version = "1.2.3",
                            ChartNameForACR = "redis",
                            ChartPathForACR = "./charts/redis",
                        },
                        Helm.Push("./charts/redis-1.2.3.tgz", "myregistry.azurecr.io") with
                        {
                            AzureSubscriptionEndpointForACR = "my-subscription",
                            AzureResourceGroupForACR = "my-resource-group",
                            AzureContainerRegistry = "myregistry.azurecr.io",
                        },
                        Helm.Login with
                        {
                            AzureSubscriptionEndpointForACR = "my-subscription",
                            AzureResourceGroupForACR = "my-resource-group",
                            AzureContainerRegistry = "myregistry.azurecr.io",
                        },
                        Helm.Logout,
                        Helm.Create("my-chart"),
                        Helm.Init with
                        {
                            CanaryImage = true,
                            UpgradeTiller = false,
                            WaitForExecution = true,
                            TillerNamespace = "tiller-namespace",
                        },
                        Helm.Ls,
                        Helm.Get,
                        Helm.Expose,
                        Helm.Delete("my-release"),
                        Helm.Uninstall("my-release"),
                        Helm.Rollback("my-release") with
                        {
                            EnableTls = true,
                            CaCert = "ca.cert",
                            Certificate = "helm.cert",
                            PrivateKey = "helm.key",
                            FailOnStderr = true,
                            PublishPipelineMetadata = false,
                        },
                    ]
                }
            ]
        };
    }

    [Fact]
    public Task Serialize_Helm_Tasks_Test()
    {
        return Verify(new HelmPipeline().Serialize());
    }

    [Fact]
    public Task Serialize_Install_Task_With_Defaults_Test()
    {
        var task = new HelmDeployInstallTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Install_Task_From_Chart_Path_Test()
    {
        var task = new HelmDeployInstallTask
        {
            ChartType = HelmChartType.FilePath,
            ChartPath = "./charts/redis",
            UseClusterAdmin = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
