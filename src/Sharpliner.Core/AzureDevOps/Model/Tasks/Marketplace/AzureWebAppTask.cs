using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Deploys an Azure App Service by using the <c>AzureWebApp@1</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-web-app-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzureWebAppV1/task.json">official AzureWebAppV1 task specification</see>.
/// </summary>
public abstract record AzureWebAppTask : AzureDevOpsTask
{
    /// <summary>
    /// Required. Azure Resource Manager service connection used for deployment.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// Required. App Service type.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<AzureWebAppType>? AppType
    {
        get => GetExpression<AzureWebAppType>("appType");
        init => SetProperty("appType", value);
    }

    /// <summary>
    /// Required. Name of the target Azure App Service.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AppName
    {
        get => GetExpression<string>("appName");
        init => SetProperty("appName", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. Deploy to a deployment slot or App Service Environment.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DeployToSlotOrAse
    {
        get => GetExpression<bool>("deployToSlotOrASE", false);
        init => SetProperty("deployToSlotOrASE", value);
    }

    /// <summary>
    /// Required when <see cref="DeployToSlotOrAse"/> is <c>true</c>. Azure resource group containing the App Service.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceGroupName
    {
        get => GetExpression<string>("resourceGroupName");
        init => SetProperty("resourceGroupName", value);
    }

    /// <summary>
    /// Required when <see cref="DeployToSlotOrAse"/> is <c>true</c>. Slot to deploy to.
    /// Default value: <c>production</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SlotName
    {
        get => GetExpression<string>("slotName", "production");
        init => SetProperty("slotName", value);
    }

    /// <summary>
    /// Required. Package path or folder to deploy.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Package
    {
        get => GetExpression<string>("package");
        init => SetProperty("package", value);
    }

    /// <summary>
    /// Optional. App settings in <c>-key value</c> syntax.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AppSettings
    {
        get => GetExpression<string>("appSettings");
        init => SetProperty("appSettings", value);
    }

    /// <summary>
    /// Optional. Configuration settings in <c>-key value</c> syntax.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConfigurationStrings
    {
        get => GetExpression<string>("configurationStrings");
        init => SetProperty("configurationStrings", value);
    }

    /// <summary>
    /// Optional. JSON configuration for SiteContainers deployments.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SiteContainersConfig
    {
        get => GetExpression<string>("siteContainersConfig");
        init => SetProperty("siteContainersConfig", value);
    }

    /// <summary>
    /// Package source category used to construct this task model.
    /// </summary>
    [YamlIgnore]
    public AzureWebAppPackageSource PackageSource { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppTask"/> class.
    /// </summary>
    protected AzureWebAppTask(AdoExpression<string> azureSubscription, AzureWebAppType appType, AdoExpression<string> appName, AdoExpression<string> package, AzureWebAppPackageSource packageSource)
        : base("AzureWebApp@1")
    {
        AzureSubscription = azureSubscription;
        AppType = appType;
        AppName = appName;
        Package = package;
        PackageSource = packageSource;
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Windows App Service with package or folder input.
/// Supports deployment method and custom web.config generation options.
/// </summary>
public record AzureWebAppWindowsPackageTask : AzureWebAppTask
{
    /// <summary>
    /// Optional. Use when <see cref="AzureWebAppTask.AppType"/> is Windows and package source is not WAR/JAR.
    /// Controls how deployment is performed.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<AzureWebAppDeploymentMethod>? DeploymentMethod
    {
        get => GetExpression<AzureWebAppDeploymentMethod>("deploymentMethod", AzureWebAppDeploymentMethod.Auto);
        init => SetProperty("deploymentMethod", value);
    }

    /// <summary>
    /// Optional. Generate web.config parameters for Python, Node.js, Go, and Java apps.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomWebConfig
    {
        get => GetExpression<string>("customWebConfig");
        init => SetProperty("customWebConfig", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppWindowsPackageTask"/> class.
    /// </summary>
    public AzureWebAppWindowsPackageTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, AzureWebAppType.WebApp, appName, package, AzureWebAppPackageSource.PackageOrFolder)
    {
        DeploymentMethod = AzureWebAppDeploymentMethod.Auto;
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Windows App Service with WAR package input.
/// </summary>
public record AzureWebAppWindowsWarTask : AzureWebAppTask
{
    /// <summary>
    /// Optional. Custom deploy folder used when package ends with <c>.war</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomDeployFolder
    {
        get => GetExpression<string>("customDeployFolder");
        init => SetProperty("customDeployFolder", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppWindowsWarTask"/> class.
    /// </summary>
    public AzureWebAppWindowsWarTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, AzureWebAppType.WebApp, appName, package, AzureWebAppPackageSource.War)
    {
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Windows App Service with JAR package input.
/// </summary>
public record AzureWebAppWindowsJarTask : AzureWebAppTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppWindowsJarTask"/> class.
    /// </summary>
    public AzureWebAppWindowsJarTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, AzureWebAppType.WebApp, appName, package, AzureWebAppPackageSource.Jar)
    {
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Linux App Service with package or folder input.
/// Supports runtime stack and startup command options.
/// </summary>
public record AzureWebAppLinuxPackageTask : AzureWebAppTask
{
    /// <summary>
    /// Optional. Runtime stack to apply for Linux App Service.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<AzureWebAppRuntimeStack>? RuntimeStack
    {
        get => GetExpression<AzureWebAppRuntimeStack>("runtimeStack");
        init => SetProperty("runtimeStack", value);
    }

    /// <summary>
    /// Optional. Startup command for Linux App Service.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? StartUpCommand
    {
        get => GetExpression<string>("startUpCommand");
        init => SetProperty("startUpCommand", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppLinuxPackageTask"/> class.
    /// </summary>
    public AzureWebAppLinuxPackageTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, AzureWebAppType.WebAppLinux, appName, package, AzureWebAppPackageSource.PackageOrFolder)
    {
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Linux App Service with WAR package input.
/// Supports runtime stack/startup command and WAR custom deploy folder.
/// </summary>
public record AzureWebAppLinuxWarTask : AzureWebAppLinuxPackageTask
{
    /// <summary>
    /// Optional. Custom deploy folder used when package ends with <c>.war</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomDeployFolder
    {
        get => GetExpression<string>("customDeployFolder");
        init => SetProperty("customDeployFolder", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppLinuxWarTask"/> class.
    /// </summary>
    public AzureWebAppLinuxWarTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, appName, package)
    {
        PackageSource = AzureWebAppPackageSource.War;
    }
}

/// <summary>
/// <c>AzureWebApp@1</c> deployment for Linux App Service with JAR package input.
/// Supports runtime stack and startup command options.
/// </summary>
public record AzureWebAppLinuxJarTask : AzureWebAppLinuxPackageTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppLinuxJarTask"/> class.
    /// </summary>
    public AzureWebAppLinuxJarTask(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package)
        : base(azureSubscription, appName, package)
    {
        PackageSource = AzureWebAppPackageSource.Jar;
    }
}

/// <summary>
/// Allowed values for <c>AzureWebApp@1</c> input <c>appType</c>.
/// </summary>
public enum AzureWebAppType
{
    /// <summary>
    /// Web App on Windows.
    /// </summary>
    [YamlMember(Alias = "webApp")]
    WebApp,

    /// <summary>
    /// Web App on Linux.
    /// </summary>
    [YamlMember(Alias = "webAppLinux")]
    WebAppLinux,
}

/// <summary>
/// Package source kinds for <c>AzureWebApp@1</c> task modeling.
/// </summary>
public enum AzureWebAppPackageSource
{
    /// <summary>
    /// Generic package path or folder.
    /// </summary>
    PackageOrFolder,

    /// <summary>
    /// WAR package.
    /// </summary>
    War,

    /// <summary>
    /// JAR package.
    /// </summary>
    Jar,
}

/// <summary>
/// Allowed values for Windows package deployment method (<c>deploymentMethod</c> input).
/// </summary>
public enum AzureWebAppDeploymentMethod
{
    /// <summary>
    /// Default. Auto-detect deployment method.
    /// </summary>
    [YamlMember(Alias = "auto")]
    Auto,

    /// <summary>
    /// Use Zip Deploy.
    /// </summary>
    [YamlMember(Alias = "zipDeploy")]
    ZipDeploy,

    /// <summary>
    /// Use Run From Package.
    /// </summary>
    [YamlMember(Alias = "runFromPackage")]
    RunFromPackage,
}

/// <summary>
/// Allowed runtime stack values for Linux <c>AzureWebApp@1</c> deployments.
/// </summary>
public enum AzureWebAppRuntimeStack
{
    /// <summary>Use the .NET Core 10.0 runtime stack.</summary>
    [YamlMember(Alias = "DOTNETCORE|10.0")]
    DotNetCore10_0,

    /// <summary>Use the .NET Core 9.0 runtime stack.</summary>
    [YamlMember(Alias = "DOTNETCORE|9.0")]
    DotNetCore9_0,

    /// <summary>Use the .NET Core 8.0 runtime stack.</summary>
    [YamlMember(Alias = "DOTNETCORE|8.0")]
    DotNetCore8_0,

    /// <summary>Use the .NET Core 7.0 runtime stack.</summary>
    [YamlMember(Alias = "DOTNETCORE|7.0")]
    DotNetCore7_0,

    /// <summary>Use the .NET Core 6.0 runtime stack.</summary>
    [YamlMember(Alias = "DOTNETCORE|6.0")]
    DotNetCore6_0,

    /// <summary>Use the Node.js 24 LTS runtime stack.</summary>
    [YamlMember(Alias = "NODE|24-lts")]
    Node24Lts,

    /// <summary>Use the Node.js 22 LTS runtime stack.</summary>
    [YamlMember(Alias = "NODE|22-lts")]
    Node22Lts,

    /// <summary>Use the Node.js 20 LTS runtime stack.</summary>
    [YamlMember(Alias = "NODE|20-lts")]
    Node20Lts,

    /// <summary>Use the Node.js 18 LTS runtime stack.</summary>
    [YamlMember(Alias = "NODE|18-lts")]
    Node18Lts,

    /// <summary>Use the Node.js 16 LTS runtime stack.</summary>
    [YamlMember(Alias = "NODE|16-lts")]
    Node16Lts,

    /// <summary>Use the Python 3.13 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.13")]
    Python3_13,

    /// <summary>Use the Python 3.12 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.12")]
    Python3_12,

    /// <summary>Use the Python 3.11 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.11")]
    Python3_11,

    /// <summary>Use the Python 3.10 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.10")]
    Python3_10,

    /// <summary>Use the Python 3.9 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.9")]
    Python3_9,

    /// <summary>Use the Python 3.8 runtime stack.</summary>
    [YamlMember(Alias = "PYTHON|3.8")]
    Python3_8,

    /// <summary>Use the PHP 8.3 runtime stack.</summary>
    [YamlMember(Alias = "PHP|8.3")]
    Php8_3,

    /// <summary>Use the PHP 8.2 runtime stack.</summary>
    [YamlMember(Alias = "PHP|8.2")]
    Php8_2,

    /// <summary>Use the PHP 8.1 runtime stack.</summary>
    [YamlMember(Alias = "PHP|8.1")]
    Php8_1,

    /// <summary>Use the PHP 8.0 runtime stack.</summary>
    [YamlMember(Alias = "PHP|8.0")]
    Php8_0,

    /// <summary>Use the Java 21 runtime stack.</summary>
    [YamlMember(Alias = "JAVA|21-java21")]
    Java21,

    /// <summary>Use the Java 17 runtime stack.</summary>
    [YamlMember(Alias = "JAVA|17-java17")]
    Java17,

    /// <summary>Use the Java 11 runtime stack.</summary>
    [YamlMember(Alias = "JAVA|11-java11")]
    Java11,

    /// <summary>Use the Java 8 runtime stack.</summary>
    [YamlMember(Alias = "JAVA|8-jre8")]
    Java8,

    /// <summary>Use JBoss EAP 8 with Java 17.</summary>
    [YamlMember(Alias = "JBOSSEAP|8-java17")]
    JbossEap8Java17,

    /// <summary>Use JBoss EAP 8 with Java 11.</summary>
    [YamlMember(Alias = "JBOSSEAP|8-java11")]
    JbossEap8Java11,

    /// <summary>Use JBoss EAP 7 with Java 17.</summary>
    [YamlMember(Alias = "JBOSSEAP|7-java17")]
    JbossEap7Java17,

    /// <summary>Use JBoss EAP 7 with Java 11.</summary>
    [YamlMember(Alias = "JBOSSEAP|7-java11")]
    JbossEap7Java11,

    /// <summary>Use JBoss EAP 7 with Java 8.</summary>
    [YamlMember(Alias = "JBOSSEAP|7-java8")]
    JbossEap7Java8,

    /// <summary>Use Tomcat 10.1 with Java 21.</summary>
    [YamlMember(Alias = "TOMCAT|10.1-java21")]
    Tomcat10_1Java21,

    /// <summary>Use Tomcat 10.1 with Java 17.</summary>
    [YamlMember(Alias = "TOMCAT|10.1-java17")]
    Tomcat10_1Java17,

    /// <summary>Use Tomcat 10.1 with Java 11.</summary>
    [YamlMember(Alias = "TOMCAT|10.1-java11")]
    Tomcat10_1Java11,

    /// <summary>Use Tomcat 10.0 with Java 17.</summary>
    [YamlMember(Alias = "TOMCAT|10.0-java17")]
    Tomcat10_0Java17,

    /// <summary>Use Tomcat 10.0 with Java 11.</summary>
    [YamlMember(Alias = "TOMCAT|10.0-java11")]
    Tomcat10_0Java11,

    /// <summary>Use Tomcat 10.0 with Java 8.</summary>
    [YamlMember(Alias = "TOMCAT|10.0-jre8")]
    Tomcat10_0Java8,

    /// <summary>Use Tomcat 9.0 with Java 21.</summary>
    [YamlMember(Alias = "TOMCAT|9.0-java21")]
    Tomcat9_0Java21,

    /// <summary>Use Tomcat 9.0 with Java 17.</summary>
    [YamlMember(Alias = "TOMCAT|9.0-java17")]
    Tomcat9_0Java17,

    /// <summary>Use Tomcat 9.0 with Java 11.</summary>
    [YamlMember(Alias = "TOMCAT|9.0-java11")]
    Tomcat9_0Java11,

    /// <summary>Use Tomcat 9.0 with Java 8.</summary>
    [YamlMember(Alias = "TOMCAT|9.0-jre8")]
    Tomcat9_0Java8,

    /// <summary>Use Tomcat 8.5 with Java 11.</summary>
    [YamlMember(Alias = "TOMCAT|8.5-java11")]
    Tomcat8_5Java11,

    /// <summary>Use Tomcat 8.5 with Java 8.</summary>
    [YamlMember(Alias = "TOMCAT|8.5-jre8")]
    Tomcat8_5Java8,
}
