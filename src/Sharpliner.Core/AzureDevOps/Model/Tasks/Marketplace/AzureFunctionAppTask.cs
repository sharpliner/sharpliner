using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base class for Azure Functions deployment tasks. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-function-app-v2?view=azure-pipelines">AzureFunctionApp task reference</see>.
/// </summary>
public abstract record AzureFunctionAppTask : AzureDevOpsTask
{
    /// <summary>Azure Resource Manager service connection.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureSubscription { get => GetExpression<string>("azureSubscription"); init => SetProperty("azureSubscription", value); }
    /// <summary>Whether the target is a Windows or Linux function app.</summary>
    [YamlIgnore] public AdoExpression<AzureFunctionAppType>? AppType { get => GetExpression<AzureFunctionAppType>("appType"); init => SetProperty("appType", value); }
    /// <summary>Azure Function App name.</summary>
    [YamlIgnore] public AdoExpression<string>? AppName { get => GetExpression<string>("appName"); init => SetProperty("appName", value); }
    /// <summary>Package or folder to deploy. The task default is <c>$(System.DefaultWorkingDirectory)/**/*.zip</c>.</summary>
    [YamlIgnore] public AdoExpression<string>? Package { get => GetExpression<string>("package"); init => SetProperty("package", value); }
    /// <summary>Deploys to a deployment slot or App Service Environment.</summary>
    [YamlIgnore] public AdoExpression<bool>? DeployToSlotOrASE { get => GetExpression<bool>("deployToSlotOrASE", false); init => SetProperty("deployToSlotOrASE", value); }
    /// <summary>Resource group, required when <see cref="DeployToSlotOrASE"/> is true.</summary>
    [YamlIgnore] public AdoExpression<string>? ResourceGroupName { get => GetExpression<string>("resourceGroupName"); init => SetProperty("resourceGroupName", value); }
    /// <summary>Slot name, required when <see cref="DeployToSlotOrASE"/> is true. The task default is <c>production</c>.</summary>
    [YamlIgnore] public AdoExpression<string>? SlotName { get => GetExpression<string>("slotName", "production"); init => SetProperty("slotName", value); }
    /// <summary>Runtime stack for Linux function apps.</summary>
    [YamlIgnore] public AdoExpression<AzureFunctionRuntimeStack>? RuntimeStack { get => GetExpression<AzureFunctionRuntimeStack>("runtimeStack"); init => SetProperty("runtimeStack", value); }
    /// <summary>App settings, one <c>-key value</c> pair per line.</summary>
    [YamlIgnore] public AdoExpression<string>? AppSettings { get => GetExpression<string>("appSettings"); init => SetProperty("appSettings", value); }
    /// <summary>Deployment method. This setting does not apply to Flex Consumption apps or WAR/JAR packages.</summary>
    [YamlIgnore] public AdoExpression<AzureFunctionDeploymentMethod>? DeploymentMethod { get => GetExpression<AzureFunctionDeploymentMethod>("deploymentMethod", AzureFunctionDeploymentMethod.Auto); init => SetProperty("deploymentMethod", value); }

    /// <summary>Initializes the common Azure Functions deployment inputs.</summary>
    protected AzureFunctionAppTask(string task, AdoExpression<string> azureSubscription, AdoExpression<AzureFunctionAppType> appType, AdoExpression<string> appName, AdoExpression<string> package)
        : base(task)
    {
        AzureSubscription = azureSubscription;
        AppType = appType;
        AppName = appName;
        Package = package;
    }
}

/// <summary>Deploys an Azure Function App using <c>AzureFunctionApp@2</c>. Version 2 supports Flex Consumption plans but removes startup-command, web.config, and configuration-string inputs. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-function-app-v2?view=azure-pipelines">official reference</see>.</summary>
public record AzureFunctionAppV2Task : AzureFunctionAppTask
{
    /// <summary>Whether the target is on a Flex Consumption plan. When true, slots, runtime stack, and deployment method are not supported by the task.</summary>
    [YamlIgnore] public AdoExpression<bool>? IsFlexConsumption { get => GetExpression<bool>("isFlexConsumption", false); init => SetProperty("isFlexConsumption", value); }

    /// <summary>Initializes an AzureFunctionApp version 2 task.</summary>
    public AzureFunctionAppV2Task(AdoExpression<string> azureSubscription, AdoExpression<AzureFunctionAppType> appType, AdoExpression<string> appName, AdoExpression<string> package)
        : base("AzureFunctionApp@2", azureSubscription, appType, appName, package) { }
}

/// <summary>Deploys an Azure Function App using <c>AzureFunctionApp@1</c>. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-function-app-v1?view=azure-pipelines">official reference</see>.</summary>
public record AzureFunctionAppV1Task : AzureFunctionAppTask
{
    /// <summary>Linux Function App startup command. Applies only when <see cref="AppType"/> is Linux.</summary>
    [YamlIgnore] public AdoExpression<string>? StartUpCommand { get => GetExpression<string>("startUpCommand"); init => SetProperty("startUpCommand", value); }
    /// <summary>Parameters used to generate web.config for Windows Python, Node.js, Go, and Java apps. Does not apply to Linux apps or WAR packages.</summary>
    [YamlIgnore] public AdoExpression<string>? CustomWebConfig { get => GetExpression<string>("customWebConfig"); init => SetProperty("customWebConfig", value); }
    /// <summary>Configuration settings, one <c>-key value</c> pair per line.</summary>
    [YamlIgnore] public AdoExpression<string>? ConfigurationStrings { get => GetExpression<string>("configurationStrings"); init => SetProperty("configurationStrings", value); }

    /// <summary>Initializes an AzureFunctionApp version 1 task.</summary>
    public AzureFunctionAppV1Task(AdoExpression<string> azureSubscription, AdoExpression<AzureFunctionAppType> appType, AdoExpression<string> appName, AdoExpression<string> package)
        : base("AzureFunctionApp@1", azureSubscription, appType, appName, package) { }
}

/// <summary>Deploys a container image to an Azure Function App using <c>AzureFunctionAppContainer@1</c>. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-function-app-container-v1?view=azure-pipelines">official reference</see>.</summary>
public record AzureFunctionAppContainerV1Task : AzureDevOpsTask
{
    /// <summary>Azure Resource Manager service connection.</summary>
    [YamlIgnore] public AdoExpression<string>? AzureSubscription { get => GetExpression<string>("azureSubscription"); init => SetProperty("azureSubscription", value); }
    /// <summary>Azure Function App name.</summary>
    [YamlIgnore] public AdoExpression<string>? AppName { get => GetExpression<string>("appName"); init => SetProperty("appName", value); }
    /// <summary>Fully qualified container image name, including registry and tag.</summary>
    [YamlIgnore] public AdoExpression<string>? ImageName { get => GetExpression<string>("imageName"); init => SetProperty("imageName", value); }
    /// <summary>Deploys to a slot or App Service Environment.</summary>
    [YamlIgnore] public AdoExpression<bool>? DeployToSlotOrASE { get => GetExpression<bool>("deployToSlotOrASE", false); init => SetProperty("deployToSlotOrASE", value); }
    /// <summary>Resource group, required when <see cref="DeployToSlotOrASE"/> is true.</summary>
    [YamlIgnore] public AdoExpression<string>? ResourceGroupName { get => GetExpression<string>("resourceGroupName"); init => SetProperty("resourceGroupName", value); }
    /// <summary>Slot name, required when <see cref="DeployToSlotOrASE"/> is true.</summary>
    [YamlIgnore] public AdoExpression<string>? SlotName { get => GetExpression<string>("slotName", "production"); init => SetProperty("slotName", value); }
    /// <summary>Container startup command.</summary>
    [YamlIgnore] public AdoExpression<string>? ContainerCommand { get => GetExpression<string>("containerCommand"); init => SetProperty("containerCommand", value); }
    /// <summary>App settings, one <c>-key value</c> pair per line.</summary>
    [YamlIgnore] public AdoExpression<string>? AppSettings { get => GetExpression<string>("appSettings"); init => SetProperty("appSettings", value); }
    /// <summary>Configuration settings, one <c>-key value</c> pair per line.</summary>
    [YamlIgnore] public AdoExpression<string>? ConfigurationStrings { get => GetExpression<string>("configurationStrings"); init => SetProperty("configurationStrings", value); }

    /// <summary>Initializes an Azure Functions container deployment task.</summary>
    public AzureFunctionAppContainerV1Task(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> imageName)
        : base("AzureFunctionAppContainer@1")
    {
        AzureSubscription = azureSubscription;
        AppName = appName;
        ImageName = imageName;
    }
}

/// <summary>Azure Function App operating system.</summary>
public enum AzureFunctionAppType
{
    /// <summary>Function App on Windows.</summary>
    [YamlMember(Alias = "functionApp")] Windows,
    /// <summary>Function App on Linux.</summary>
    [YamlMember(Alias = "functionAppLinux")] Linux,
}

/// <summary>Azure Function App deployment method.</summary>
public enum AzureFunctionDeploymentMethod
{
    /// <summary>Automatically select the deployment method.</summary>
    [YamlMember(Alias = "auto")] Auto,
    /// <summary>Deploy the ZIP package.</summary>
    [YamlMember(Alias = "zipDeploy")] ZipDeploy,
    /// <summary>Deploy the ZIP package using Run From Package.</summary>
    [YamlMember(Alias = "runFromPackage")] RunFromPackage,
}

/// <summary>Supported Linux Function App runtime stacks.</summary>
public enum AzureFunctionRuntimeStack
{
    /// <summary>.NET 6 in-process.</summary>
    [YamlMember(Alias = "DOTNET|6.0")] DotNet6,
    /// <summary>.NET 6 isolated worker.</summary>
    [YamlMember(Alias = "DOTNET-ISOLATED|6.0")] DotNetIsolated6,
    /// <summary>.NET 7 isolated worker.</summary>
    [YamlMember(Alias = "DOTNET-ISOLATED|7.0")] DotNetIsolated7,
    /// <summary>.NET 8 isolated worker.</summary>
    [YamlMember(Alias = "DOTNET-ISOLATED|8.0")] DotNetIsolated8,
    /// <summary>.NET 9 isolated worker.</summary>
    [YamlMember(Alias = "DOTNET-ISOLATED|9.0")] DotNetIsolated9,
    /// <summary>.NET 10 isolated worker.</summary>
    [YamlMember(Alias = "DOTNET-ISOLATED|10.0")] DotNetIsolated10,
    /// <summary>Java 8.</summary>
    [YamlMember(Alias = "JAVA|8")] Java8,
    /// <summary>Java 11.</summary>
    [YamlMember(Alias = "JAVA|11")] Java11,
    /// <summary>Java 17.</summary>
    [YamlMember(Alias = "JAVA|17")] Java17,
    /// <summary>Java 21.</summary>
    [YamlMember(Alias = "JAVA|21")] Java21,
    /// <summary>Java 25.</summary>
    [YamlMember(Alias = "JAVA|25")] Java25,
    /// <summary>Node.js 14.</summary>
    [YamlMember(Alias = "NODE|14")] Node14,
    /// <summary>Node.js 16.</summary>
    [YamlMember(Alias = "NODE|16")] Node16,
    /// <summary>Node.js 18.</summary>
    [YamlMember(Alias = "NODE|18")] Node18,
    /// <summary>Node.js 20.</summary>
    [YamlMember(Alias = "NODE|20")] Node20,
    /// <summary>Node.js 22.</summary>
    [YamlMember(Alias = "NODE|22")] Node22,
    /// <summary>Node.js 24.</summary>
    [YamlMember(Alias = "NODE|24")] Node24,
    /// <summary>Python 3.8.</summary>
    [YamlMember(Alias = "PYTHON|3.8")] Python38,
    /// <summary>Python 3.9.</summary>
    [YamlMember(Alias = "PYTHON|3.9")] Python39,
    /// <summary>Python 3.10.</summary>
    [YamlMember(Alias = "PYTHON|3.10")] Python310,
    /// <summary>Python 3.11.</summary>
    [YamlMember(Alias = "PYTHON|3.11")] Python311,
    /// <summary>Python 3.12.</summary>
    [YamlMember(Alias = "PYTHON|3.12")] Python312,
    /// <summary>Python 3.13.</summary>
    [YamlMember(Alias = "PYTHON|3.13")] Python313,
}
