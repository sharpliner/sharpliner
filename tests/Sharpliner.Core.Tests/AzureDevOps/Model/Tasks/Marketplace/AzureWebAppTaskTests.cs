using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureWebAppTaskTests
{
    [Fact]
    public Task Serialize_Windows_Package_Task_Test()
    {
        var task = new AzureWebAppWindowsPackageTask("my-azure-connection", "my-windows-app", "$(System.DefaultWorkingDirectory)/**/*.zip")
        {
            DeploymentMethod = AzureWebAppDeploymentMethod.RunFromPackage,
            DeployToSlotOrAse = true,
            ResourceGroupName = "my-resource-group",
            SlotName = "staging",
            CustomWebConfig = "-appType node -startupFile server.js",
            AppSettings = "-WEBSITE_TIME_ZONE \"Eastern Standard Time\"",
            ConfigurationStrings = "-linuxFxVersion node|20-lts",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Windows_War_Task_Test()
    {
        var task = new AzureWebAppWindowsWarTask("my-azure-connection", "my-windows-app", "$(System.DefaultWorkingDirectory)/**/*.war")
        {
            CustomDeployFolder = "ROOT",
            AppSettings = "-Port 5000",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Linux_Package_Task_Test()
    {
        var task = new AzureWebAppLinuxPackageTask("my-azure-connection", "my-linux-app", "$(System.DefaultWorkingDirectory)/**/*.zip")
        {
            RuntimeStack = AzureWebAppRuntimeStack.Node20Lts,
            StartUpCommand = "npm run start",
            SiteContainersConfig = "{\"containers\":[]}",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Linux_War_Task_Test()
    {
        var task = new AzureWebAppLinuxWarTask("my-azure-connection", "my-linux-app", "$(System.DefaultWorkingDirectory)/**/*.war")
        {
            RuntimeStack = AzureWebAppRuntimeStack.Java17,
            StartUpCommand = "java -jar app.war",
            CustomDeployFolder = "ROOT",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
