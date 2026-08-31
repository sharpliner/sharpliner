using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating Azure App Service deployment tasks using the <c>AzureAppServiceDeploy</c> keyword.
/// The generated YAML uses the <c>AzureRmWebAppDeployment@5</c> task as defined by the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-rm-web-app-deployment-v5?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// Each method returns a task with a valid combination of the connection type, the app type and the required inputs.
/// </summary>
public class AzureAppServiceDeployTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Deploys a package or a folder to a Web App on Windows (<c>appType: webApp</c>).
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureAppServiceDeploy.WebApp("myServiceConnection", "my-app", "$(Build.ArtifactStagingDirectory)/**/*.zip", "Deploy web app")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureRmWebAppDeployment@5
    ///   displayName: Deploy web app
    ///   inputs:
    ///     ConnectionType: AzureRM
    ///     azureSubscription: myServiceConnection
    ///     WebAppName: my-app
    ///     appType: webApp
    ///     packageForLinux: $(Build.ArtifactStagingDirectory)/**/*.zip
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager subscription (service connection) used for the deployment.</param>
    /// <param name="webAppName">Name of an existing Azure App Service.</param>
    /// <param name="package">File path to the package or a folder containing the App Service contents.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureRmWebAppDeploymentPackageTask"/> deploying a Windows web app.</returns>
    public AzureRmWebAppDeploymentPackageTask WebApp(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> webAppName,
        AdoExpression<string> package,
        AdoExpression<string>? displayName = null)
        => new AzureRmWebAppDeploymentPackageTask(azureSubscription, webAppName, package) with
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Deploys a package or a folder to a Web App on Linux (<c>appType: webAppLinux</c>).
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager subscription (service connection) used for the deployment.</param>
    /// <param name="webAppName">Name of an existing Azure App Service.</param>
    /// <param name="package">File path to the package or a folder containing the App Service contents.</param>
    /// <param name="runtimeStack">Framework and version of the web app, for example <c>DOTNETCORE|8.0</c>.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureRmWebAppDeploymentPackageTask"/> deploying a Linux web app.</returns>
    public AzureRmWebAppDeploymentPackageTask WebAppLinux(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> webAppName,
        AdoExpression<string> package,
        AdoExpression<string>? runtimeStack = null,
        AdoExpression<string>? displayName = null)
        => new AzureRmWebAppDeploymentPackageTask(azureSubscription, webAppName, package, AzureAppServicePackageAppType.WebAppLinux) with
        {
            RuntimeStack = runtimeStack,
            DisplayName = displayName!,
        };

    /// <summary>
    /// Deploys a package or a folder to an App Service of the given type.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager subscription (service connection) used for the deployment.</param>
    /// <param name="webAppName">Name of an existing Azure App Service.</param>
    /// <param name="package">File path to the package or a folder containing the App Service contents.</param>
    /// <param name="appType">Type of the App Service the package is deployed to.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureRmWebAppDeploymentPackageTask"/>.</returns>
    public AzureRmWebAppDeploymentPackageTask Package(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> webAppName,
        AdoExpression<string> package,
        AzureAppServicePackageAppType appType,
        AdoExpression<string>? displayName = null)
        => new AzureRmWebAppDeploymentPackageTask(azureSubscription, webAppName, package, appType) with
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Deploys a container image to a Web App or a Function App for Containers.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureAppServiceDeploy.Container("myServiceConnection", "my-app", "myregistry.azurecr.io", "nginx", displayName: "Deploy container")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureRmWebAppDeployment@5
    ///   displayName: Deploy container
    ///   inputs:
    ///     ConnectionType: AzureRM
    ///     azureSubscription: myServiceConnection
    ///     WebAppName: my-app
    ///     appType: webAppContainer
    ///     DockerNamespace: myregistry.azurecr.io
    ///     DockerRepository: nginx
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager subscription (service connection) used for the deployment.</param>
    /// <param name="webAppName">Name of an existing Azure App Service.</param>
    /// <param name="dockerNamespace">Globally unique top-level domain name of the registry or namespace.</param>
    /// <param name="dockerRepository">Name of the repository where the container images are stored.</param>
    /// <param name="dockerImageTag">Tag of the container image, for example <c>latest</c>.</param>
    /// <param name="appType">Type of the App Service the container image is deployed to.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureRmWebAppDeploymentContainerTask"/>.</returns>
    public AzureRmWebAppDeploymentContainerTask Container(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> webAppName,
        AdoExpression<string> dockerNamespace,
        AdoExpression<string> dockerRepository,
        AdoExpression<string>? dockerImageTag = null,
        AzureAppServiceContainerAppType appType = AzureAppServiceContainerAppType.WebAppContainer,
        AdoExpression<string>? displayName = null)
        => new AzureRmWebAppDeploymentContainerTask(azureSubscription, webAppName, dockerNamespace, dockerRepository, appType) with
        {
            DockerImageTag = dockerImageTag,
            DisplayName = displayName!,
        };

    /// <summary>
    /// Deploys a package using a publish profile created in Visual Studio (<c>ConnectionType: PublishProfile</c>).
    /// </summary>
    /// <param name="publishProfilePath">Path of the publish profile created in Visual Studio.</param>
    /// <param name="publishProfilePassword">Password of the publish profile, ideally stored in a secret variable.</param>
    /// <param name="package">File path to the package or a folder containing the App Service contents.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureRmWebAppDeploymentPublishProfileTask"/>.</returns>
    public AzureRmWebAppDeploymentPublishProfileTask PublishProfile(
        AdoExpression<string> publishProfilePath,
        AdoExpression<string> publishProfilePassword,
        AdoExpression<string> package,
        AdoExpression<string>? displayName = null)
        => new AzureRmWebAppDeploymentPublishProfileTask(publishProfilePath, publishProfilePassword, package) with
        {
            DisplayName = displayName!,
        };

    internal AzureAppServiceDeployTaskBuilder()
    {
    }
}
