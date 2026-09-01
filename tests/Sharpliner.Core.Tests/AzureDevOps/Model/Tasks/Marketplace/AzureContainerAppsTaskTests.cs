using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureContainerAppsTaskTests
{
    [Fact]
    public Task Serialize_V1_Source_Task_With_Advanced_Inputs_Test()
    {
        var task = new AzureContainerAppsV1FromSourceTask("my-azure-connection", "$(Build.SourcesDirectory)/src/MyApp", "myregistry")
        {
            AcrUsername = "registry-user",
            AcrPassword = "$(registryPassword)",
            DockerfilePath = "deploy/Dockerfile",
            ImageToBuild = "myregistry.azurecr.io/my-app:$(Build.BuildId)",
            ImageToDeploy = "myregistry.azurecr.io/my-app:stable",
            RuntimeStack = "dotnet:8.0",
            ContainerAppName = "my-container-app",
            ResourceGroup = "rg-container-apps",
            ContainerAppEnvironment = "my-ca-env",
            TargetPort = "8080",
            Location = "westeurope",
            EnvironmentVariables = "ASPNETCORE_ENVIRONMENT=Production API_KEY=secretref:apiKey",
            Ingress = AzureContainerAppIngress.External,
            Kind = AzureContainerAppKind.FunctionApp,
            DisableTelemetry = true,
            WorkingDirectory = "$(System.DefaultWorkingDirectory)"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V1_Image_Task_Test()
    {
        var task = new AzureContainerAppsV1FromImageTask("my-azure-connection", "myregistry.azurecr.io/my-app:1.2.3")
        {
            AcrName = "myregistry",
            AcrUsername = "registry-user",
            AcrPassword = "$(registryPassword)",
            ContainerAppName = "my-container-app",
            ResourceGroup = "rg-container-apps",
            Ingress = AzureContainerAppIngress.Internal
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V1_Yaml_Task_Test()
    {
        var task = new AzureContainerAppsV1FromYamlTask("my-azure-connection", "$(System.DefaultWorkingDirectory)/infra/containerapp.yaml")
        {
            ContainerAppName = "my-container-app",
            DisableTelemetry = false,
            Ingress = AzureContainerAppIngress.Disabled
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V0_Image_Task_Test()
    {
#pragma warning disable CS0618
        var task = new AzureContainerAppsV0FromImageTask("my-azure-connection", "myregistry.azurecr.io/my-app:legacy")
        {
            ContainerAppName = "legacy-container-app"
        };
#pragma warning restore CS0618

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
