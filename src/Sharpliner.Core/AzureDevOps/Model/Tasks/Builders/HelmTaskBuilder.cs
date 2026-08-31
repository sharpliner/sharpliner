using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create the <c>HelmDeploy@1</c> tasks in Azure DevOps pipelines.
/// Each Helm command is represented by a dedicated task record which only exposes the inputs that are valid for that command.
/// See the official <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">HelmDeploy@1 task reference</see>.
/// </summary>
public class HelmTaskBuilder
{
    /// <summary>
    /// <para>
    /// Gets a <see cref="HelmInstallBuilder"/> instance to create <c>helm install</c> tasks.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Helm.Install.FromChartName("stable/mysql") with
    /// {
    ///     ReleaseName = "my-release"
    /// }
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: install
    ///     chartType: Name
    ///     chartName: stable/mysql
    ///     releaseName: my-release
    /// </code>
    /// </summary>
    public HelmInstallBuilder Install => new();

    /// <summary>
    /// <para>
    /// Gets a <see cref="HelmUpgradeBuilder"/> instance to create <c>helm upgrade</c> tasks.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Helm.Upgrade.FromChartPath("./charts/redis") with
    /// {
    ///     ReleaseName = "my-release"
    /// }
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: upgrade
    ///     chartType: FilePath
    ///     chartPath: ./charts/redis
    ///     releaseName: my-release
    /// </code>
    /// </summary>
    public HelmUpgradeBuilder Upgrade => new();

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployInitTask"/> which initializes Helm and installs Tiller in the cluster.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Init
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: init
    /// </code>
    /// </summary>
    public HelmDeployInitTask Init => new();

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployLsTask"/> which lists the releases in the cluster.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Ls
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: ls
    /// </code>
    /// </summary>
    public HelmDeployLsTask Ls => new();

    /// <summary>
    /// Creates a <see cref="HelmDeployGetTask"/> which downloads the information of a named release.
    /// </summary>
    public HelmDeployGetTask Get => new();

    /// <summary>
    /// Creates a <see cref="HelmDeployExposeTask"/> which exposes a release through a Kubernetes service.
    /// </summary>
    public HelmDeployExposeTask Expose => new();

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployLoginTask"/> which logs in to an Azure Container Registry.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Login with
    /// {
    ///     AzureSubscriptionEndpointForACR = "my-subscription",
    ///     AzureResourceGroupForACR = "my-resource-group",
    ///     AzureContainerRegistry = "myregistry.azurecr.io"
    /// }
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: login
    ///     azureSubscriptionEndpointForACR: my-subscription
    ///     azureResourceGroupForACR: my-resource-group
    ///     azureContainerRegistry: myregistry.azurecr.io
    /// </code>
    /// </summary>
    public HelmDeployLoginTask Login => new();

    /// <summary>
    /// Creates a <see cref="HelmDeployLogoutTask"/> which logs out of an Azure Container Registry.
    /// </summary>
    public HelmDeployLogoutTask Logout => new();

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployCreateTask"/> which creates a new chart with the given name.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Create("my-chart")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: create
    ///     chartName: my-chart
    /// </code>
    /// </summary>
    /// <param name="chartName">The name of the chart to create.</param>
    /// <returns>A <see cref="HelmDeployCreateTask"/> instance.</returns>
    public HelmDeployCreateTask Create(AdoExpression<string> chartName) => new(chartName);

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployPackageTask"/> which packages a chart directory into a chart archive.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Package("./charts/redis") with
    /// {
    ///     Destination = "$(Build.ArtifactStagingDirectory)"
    /// }
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: package
    ///     chartPath: ./charts/redis
    ///     destination: $(Build.ArtifactStagingDirectory)
    /// </code>
    /// </summary>
    /// <param name="chartPath">The path to the chart directory to package.</param>
    /// <returns>A <see cref="HelmDeployPackageTask"/> instance.</returns>
    public HelmDeployPackageTask Package(AdoExpression<string> chartPath) => new() { ChartPath = chartPath };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployPushTask"/> which pushes a chart to a remote repository.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Push("./charts/redis-1.0.0.tgz", "myregistry.azurecr.io")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: push
    ///     chartPath: ./charts/redis-1.0.0.tgz
    ///     remoteRepo: myregistry.azurecr.io
    /// </code>
    /// </summary>
    /// <param name="chartPath">The path to the chart to push.</param>
    /// <param name="remoteRepo">The remote repository the chart is pushed to.</param>
    /// <returns>A <see cref="HelmDeployPushTask"/> instance.</returns>
    public HelmDeployPushTask Push(AdoExpression<string> chartPath, AdoExpression<string> remoteRepo) => new()
    {
        ChartPath = chartPath,
        RemoteRepo = remoteRepo,
    };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployDeleteTask"/> which deletes a release from the cluster.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Delete("my-release")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: delete
    ///     releaseName: my-release
    /// </code>
    /// </summary>
    /// <param name="releaseName">The name of the release to delete.</param>
    /// <returns>A <see cref="HelmDeployDeleteTask"/> instance.</returns>
    public HelmDeployDeleteTask Delete(AdoExpression<string> releaseName) => new() { ReleaseName = releaseName };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployUninstallTask"/> which uninstalls a release from the cluster.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Uninstall("my-release")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: uninstall
    ///     releaseName: my-release
    /// </code>
    /// </summary>
    /// <param name="releaseName">The name of the release to uninstall.</param>
    /// <returns>A <see cref="HelmDeployUninstallTask"/> instance.</returns>
    public HelmDeployUninstallTask Uninstall(AdoExpression<string> releaseName) => new() { ReleaseName = releaseName };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployRollbackTask"/> which rolls a release back to a previous revision.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Rollback("my-release")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: rollback
    ///     releaseName: my-release
    /// </code>
    /// </summary>
    /// <param name="releaseName">The name of the release to roll back.</param>
    /// <returns>A <see cref="HelmDeployRollbackTask"/> instance.</returns>
    public HelmDeployRollbackTask Rollback(AdoExpression<string> releaseName) => new() { ReleaseName = releaseName };
}

/// <summary>
/// Provides methods to create <c>helm install</c> tasks.
/// The chart can either be referenced by its name (<c>chartType: Name</c>) or by a path (<c>chartType: FilePath</c>).
/// </summary>
public class HelmInstallBuilder
{
    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployInstallTask"/> which installs a chart referenced by its name.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Install.FromChartName("stable/mysql")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: install
    ///     chartType: Name
    ///     chartName: stable/mysql
    /// </code>
    /// </summary>
    /// <param name="chartName">The chart reference to install, which can be a URL or a chart name such as <c>stable/mysql</c>.</param>
    /// <returns>A <see cref="HelmDeployInstallTask"/> instance.</returns>
    public HelmDeployInstallTask FromChartName(AdoExpression<string> chartName) => new()
    {
        ChartType = HelmChartType.Name,
        ChartName = chartName,
    };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployInstallTask"/> which installs a chart referenced by a path.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Install.FromChartPath("./charts/redis")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: install
    ///     chartType: FilePath
    ///     chartPath: ./charts/redis
    /// </code>
    /// </summary>
    /// <param name="chartPath">The path to a packaged chart or to an unpacked chart directory.</param>
    /// <returns>A <see cref="HelmDeployInstallTask"/> instance.</returns>
    public HelmDeployInstallTask FromChartPath(AdoExpression<string> chartPath) => new()
    {
        ChartType = HelmChartType.FilePath,
        ChartPath = chartPath,
    };
}

/// <summary>
/// Provides methods to create <c>helm upgrade</c> tasks.
/// The chart can either be referenced by its name (<c>chartType: Name</c>) or by a path (<c>chartType: FilePath</c>).
/// </summary>
public class HelmUpgradeBuilder
{
    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployUpgradeTask"/> which upgrades a release using a chart referenced by its name.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Upgrade.FromChartName("stable/mysql")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: upgrade
    ///     chartType: Name
    ///     chartName: stable/mysql
    /// </code>
    /// </summary>
    /// <param name="chartName">The chart reference to upgrade to, which can be a URL or a chart name such as <c>stable/mysql</c>.</param>
    /// <returns>A <see cref="HelmDeployUpgradeTask"/> instance.</returns>
    public HelmDeployUpgradeTask FromChartName(AdoExpression<string> chartName) => new()
    {
        ChartType = HelmChartType.Name,
        ChartName = chartName,
    };

    /// <summary>
    /// <para>
    /// Creates a <see cref="HelmDeployUpgradeTask"/> which upgrades a release using a chart referenced by a path.
    /// </para>
    /// <code lang="csharp">
    /// Helm.Upgrade.FromChartPath("./charts/redis")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: HelmDeploy@1
    ///   inputs:
    ///     command: upgrade
    ///     chartType: FilePath
    ///     chartPath: ./charts/redis
    /// </code>
    /// </summary>
    /// <param name="chartPath">The path to a packaged chart or to an unpacked chart directory.</param>
    /// <returns>A <see cref="HelmDeployUpgradeTask"/> instance.</returns>
    public HelmDeployUpgradeTask FromChartPath(AdoExpression<string> chartPath) => new()
    {
        ChartType = HelmChartType.FilePath,
        ChartPath = chartPath,
    };
}
