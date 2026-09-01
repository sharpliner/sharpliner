using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureRmWebAppDeploymentTaskTests
{
    [Fact]
    public Task Serialize_WebApp_Task_With_Defaults_Test()
    {
        var task = new AzureRmWebAppDeploymentPackageTask("my-subscription", "my-app", "$(Build.ArtifactStagingDirectory)/**/*.zip");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_WebApp_Task_With_WebDeploy_Options_Test()
    {
        var task = new AzureRmWebAppDeploymentPackageTask("my-subscription", "my-app", "$(Build.ArtifactStagingDirectory)/**/*.zip")
        {
            DeployToSlotOrASE = true,
            ResourceGroupName = "my-resource-group",
            SlotName = "staging",
            VirtualApplication = "my-virtual-app",
            AppSettings = "-Port 5000 -WEBSITE_TIME_ZONE \"Eastern Standard Time\"",
            ConfigurationSettings = "-phpVersion 5.6",
            UseCustomDeployment = true,
            DeploymentMethod = AzureAppServiceDeploymentMethod.WebDeploy,
            TakeAppOffline = true,
            SetParametersFile = "SetParameters.xml",
            RemoveAdditionalFiles = true,
            ExcludeFilesFromAppData = false,
            AdditionalArguments = "-disableLink:AppPoolExtension",
            RenameFiles = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_WebApp_Task_With_Transformations_Test()
    {
        var task = new AzureRmWebAppDeploymentPackageTask("my-subscription", "my-app", "$(Build.ArtifactStagingDirectory)/**/*.zip")
        {
            WebConfigParameters = "-Handler iisnode -NodeStartFile server.js -appType node",
            XmlTransformation = true,
            XmlVariableSubstitution = true,
            JsonFiles = "appsettings.json",
            ScriptType = AzureAppServiceDeploymentScriptType.InlineScript,
            InlineScript = "npm install",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_WebAppLinux_Task_Test()
    {
        var task = new AzureRmWebAppDeploymentPackageTask(
            "my-subscription",
            "my-app",
            "$(Build.ArtifactStagingDirectory)/**/*.zip",
            AzureAppServicePackageAppType.WebAppLinux)
        {
            RuntimeStack = "DOTNETCORE|8.0",
            StartupCommand = "dotnet exec MyApp.dll",
            DeploymentMethodLinux = AzureAppServiceLinuxDeploymentMethod.OneDeploy,
            CleanDeployment = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FunctionAppLinux_Task_Test()
    {
        var task = new AzureRmWebAppDeploymentPackageTask(
            "my-subscription",
            "my-function-app",
            "$(Build.ArtifactStagingDirectory)/**/*.zip",
            AzureAppServicePackageAppType.FunctionAppLinux)
        {
            RuntimeStackFunction = "NODE|22",
            ScriptType = AzureAppServiceDeploymentScriptType.FilePath,
            ScriptPath = "scripts/post-deploy.sh",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Container_Task_Test()
    {
        var task = new AzureRmWebAppDeploymentContainerTask(
            "my-subscription",
            "my-app",
            "myregistry.azurecr.io",
            "nginx",
            AzureAppServiceContainerAppType.WebAppHyperVContainer)
        {
            DockerImageTag = "$(Build.BuildId)",
            StartupCommand = "dotnet exec MyApp.dll",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_PublishProfile_Task_Test()
    {
        var task = new AzureRmWebAppDeploymentPublishProfileTask(
            "$(System.DefaultWorkingDirectory)/**/*.pubxml",
            "$(PublishProfilePassword)",
            "$(Build.ArtifactStagingDirectory)/**/*.zip")
        {
            XmlVariableSubstitution = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
