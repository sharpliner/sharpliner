using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides a fluent API for creating valid <c>AzureWebApp@1</c> deployment task combinations.
/// </summary>
public class AzureWebAppTaskBuilder
{
    /// <summary>
    /// Starts configuration for a Windows App Service deployment.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection.</param>
    /// <param name="appName">Target App Service name.</param>
    public AzureWebAppWindowsBuilder Windows(AdoExpression<string> azureSubscription, AdoExpression<string> appName)
    {
        return new(azureSubscription, appName);
    }

    /// <summary>
    /// Starts configuration for a Linux App Service deployment.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection.</param>
    /// <param name="appName">Target App Service name.</param>
    public AzureWebAppLinuxBuilder Linux(AdoExpression<string> azureSubscription, AdoExpression<string> appName)
    {
        return new(azureSubscription, appName);
    }
}

/// <summary>
/// Fluent builder for Windows <c>AzureWebApp@1</c> task variants.
/// </summary>
public class AzureWebAppWindowsBuilder
{
    private readonly AdoExpression<string> _azureSubscription;
    private readonly AdoExpression<string> _appName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppWindowsBuilder"/> class.
    /// </summary>
    public AzureWebAppWindowsBuilder(AdoExpression<string> azureSubscription, AdoExpression<string> appName)
    {
        System.ArgumentNullException.ThrowIfNull(azureSubscription);
        System.ArgumentNullException.ThrowIfNull(appName);
        _azureSubscription = azureSubscription;
        _appName = appName;
    }

    /// <summary>
    /// Creates a Windows package/folder deployment task.
    /// Supports deployment method and generated web.config options.
    /// </summary>
    public AzureWebAppWindowsPackageTask Package(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }

    /// <summary>
    /// Creates a Windows WAR deployment task.
    /// </summary>
    public AzureWebAppWindowsWarTask War(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }

    /// <summary>
    /// Creates a Windows JAR deployment task.
    /// </summary>
    public AzureWebAppWindowsJarTask Jar(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }
}

/// <summary>
/// Fluent builder for Linux <c>AzureWebApp@1</c> task variants.
/// </summary>
public class AzureWebAppLinuxBuilder
{
    private readonly AdoExpression<string> _azureSubscription;
    private readonly AdoExpression<string> _appName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebAppLinuxBuilder"/> class.
    /// </summary>
    public AzureWebAppLinuxBuilder(AdoExpression<string> azureSubscription, AdoExpression<string> appName)
    {
        System.ArgumentNullException.ThrowIfNull(azureSubscription);
        System.ArgumentNullException.ThrowIfNull(appName);
        _azureSubscription = azureSubscription;
        _appName = appName;
    }

    /// <summary>
    /// Creates a Linux package/folder deployment task.
    /// Supports runtime stack and startup command options.
    /// </summary>
    public AzureWebAppLinuxPackageTask Package(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }

    /// <summary>
    /// Creates a Linux WAR deployment task.
    /// </summary>
    public AzureWebAppLinuxWarTask War(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }

    /// <summary>
    /// Creates a Linux JAR deployment task.
    /// </summary>
    public AzureWebAppLinuxJarTask Jar(AdoExpression<string> package)
    {
        System.ArgumentNullException.ThrowIfNull(package);
        return new(_azureSubscription, _appName, package);
    }
}
