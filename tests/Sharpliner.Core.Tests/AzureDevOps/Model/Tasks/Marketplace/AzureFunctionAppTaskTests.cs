using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureFunctionAppTaskTests
{
    [Fact]
    public Task Serialize_V2Linux_With_ConditionalInputs_Test()
    {
        var task = new AzureFunctionAppV2Task("azure", AzureFunctionAppType.Linux, "function", "$(Pipeline.Workspace)/app.zip")
        {
            DeployToSlotOrASE = true,
            ResourceGroupName = "rg",
            SlotName = "staging",
            RuntimeStack = AzureFunctionRuntimeStack.DotNetIsolated10,
            AppSettings = "-WEBSITE_RUN_FROM_PACKAGE 1",
            DeploymentMethod = AzureFunctionDeploymentMethod.RunFromPackage,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V1AndContainer_ConditionalInputs_Test()
    {
        var tasks = new AdoExpressionList<Step>
        {
            new AzureFunctionAppV1Task("azure", AzureFunctionAppType.Windows, "function", "app.zip")
            {
                CustomWebConfig = "-handler iisnode",
                ConfigurationStrings = "-Database server",
                DeploymentMethod = AzureFunctionDeploymentMethod.ZipDeploy,
            },
            new AzureFunctionAppContainerV1Task("azure", "function", "registry.azurecr.io/function:v1")
            {
                DeployToSlotOrASE = true,
                ResourceGroupName = "rg",
                SlotName = "staging",
                ContainerCommand = "dotnet Function.dll",
                AppSettings = "-Key Value",
                ConfigurationStrings = "-Connection connection",
            },
        };

        return Verify(SharplinerSerializer.Serialize(tasks));
    }

    [Fact]
    public Task Serialize_Builder_ValidCombinations_Test()
        => Verify(new AzureFunctionAppPipeline().Serialize());

    private class AzureFunctionAppPipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            AzureFunctionApp.Windows("azure", "windows-function", "app.zip", AzureFunctionDeploymentMethod.ZipDeploy),
            AzureFunctionApp.Linux("azure", "linux-function", "app.zip", AzureFunctionRuntimeStack.Python313),
            AzureFunctionApp.FlexConsumption("azure", AzureFunctionAppType.Linux, "flex-function", "app.zip"),
            AzureFunctionApp.V1Linux("azure", "v1-function", "app.zip", AzureFunctionRuntimeStack.Node24),
            AzureFunctionApp.Container("azure", "container-function", "registry.azurecr.io/function:v1"),
        ];
    }
}
