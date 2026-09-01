using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create various npm tasks in Azure DevOps pipelines.
/// </summary>
public class NpmTaskBuilder
{
    /// <summary>
    /// Creates an <see cref="NpmInstallTask"/> that runs <c>npm install</c> with registries from the repository <c>.npmrc</c>.
    /// </summary>
    /// <param name="customEndpoints">Optional npm service connection names for registries outside this organization/collection.</param>
    /// <returns>An <see cref="NpmInstallTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// Npm.Install(["myExternalRegistryServiceConnection"])
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: Npm@1
    ///   inputs:
    ///     command: install
    ///     customRegistry: useNpmrc
    ///     customEndpoint: myExternalRegistryServiceConnection
    /// </code>
    /// </example>
    public NpmInstallTask Install(string[]? customEndpoints = null) => new()
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc,
        CustomEndpoint = customEndpoints,
    };

    /// <summary>
    /// Creates an <see cref="NpmInstallTask"/> that runs <c>npm install</c> using a selected Azure Artifacts/TFS feed.
    /// </summary>
    /// <param name="customFeed">The feed to include in the generated <c>.npmrc</c> file.</param>
    /// <returns>An <see cref="NpmInstallTask"/> instance.</returns>
    public NpmInstallTask InstallFromFeed(AdoExpression<string> customFeed) => new()
    {
        CustomRegistry = NpmCustomRegistry.UseFeed,
        CustomFeed = customFeed,
    };

    /// <summary>
    /// Creates an <see cref="NpmCiTask"/> that runs <c>npm ci</c> with registries from the repository <c>.npmrc</c>.
    /// </summary>
    /// <param name="customEndpoints">Optional npm service connection names for registries outside this organization/collection.</param>
    /// <returns>An <see cref="NpmCiTask"/> instance.</returns>
    public NpmCiTask Ci(string[]? customEndpoints = null) => new()
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc,
        CustomEndpoint = customEndpoints,
    };

    /// <summary>
    /// Creates an <see cref="NpmCiTask"/> that runs <c>npm ci</c> using a selected Azure Artifacts/TFS feed.
    /// </summary>
    /// <param name="customFeed">The feed to include in the generated <c>.npmrc</c> file.</param>
    /// <returns>An <see cref="NpmCiTask"/> instance.</returns>
    public NpmCiTask CiFromFeed(AdoExpression<string> customFeed) => new()
    {
        CustomRegistry = NpmCustomRegistry.UseFeed,
        CustomFeed = customFeed,
    };

    /// <summary>
    /// Creates an <see cref="NpmCustomTask"/> that runs a custom npm command with registries from the repository <c>.npmrc</c>.
    /// </summary>
    /// <param name="customCommand">The custom npm command and arguments, such as <c>dist-tag ls mypackage</c>.</param>
    /// <param name="customEndpoints">Optional npm service connection names for registries outside this organization/collection.</param>
    /// <returns>An <see cref="NpmCustomTask"/> instance.</returns>
    public NpmCustomTask Custom(AdoExpression<string> customCommand, string[]? customEndpoints = null) => new(customCommand)
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc,
        CustomEndpoint = customEndpoints,
    };

    /// <summary>
    /// Creates an <see cref="NpmCustomTask"/> that runs a custom npm command using a selected Azure Artifacts/TFS feed.
    /// </summary>
    /// <param name="customCommand">The custom npm command and arguments, such as <c>dist-tag ls mypackage</c>.</param>
    /// <param name="customFeed">The feed to include in the generated <c>.npmrc</c> file.</param>
    /// <returns>An <see cref="NpmCustomTask"/> instance.</returns>
    public NpmCustomTask CustomFromFeed(AdoExpression<string> customCommand, AdoExpression<string> customFeed) => new(customCommand)
    {
        CustomRegistry = NpmCustomRegistry.UseFeed,
        CustomFeed = customFeed,
    };

    /// <summary>
    /// Creates an <see cref="NpmPublishTask"/> that runs <c>npm publish</c> to an external npm registry.
    /// </summary>
    /// <param name="publishEndpoint">The npm service connection used to publish to the external registry.</param>
    /// <returns>An <see cref="NpmPublishTask"/> instance.</returns>
    public NpmPublishTask PublishToExternalRegistry(AdoExpression<string> publishEndpoint) => new()
    {
        PublishRegistry = NpmPublishRegistry.UseExternalRegistry,
        PublishEndpoint = publishEndpoint,
    };

    /// <summary>
    /// Creates an <see cref="NpmPublishTask"/> that runs <c>npm publish</c> to an Azure Artifacts/TFS feed.
    /// </summary>
    /// <param name="publishFeed">The feed to which npm packages are published.</param>
    /// <returns>An <see cref="NpmPublishTask"/> instance.</returns>
    public NpmPublishTask PublishToFeed(AdoExpression<string> publishFeed) => new()
    {
        PublishRegistry = NpmPublishRegistry.UseFeed,
        PublishFeed = publishFeed,
    };

    /// <summary>
    /// Creates an <see cref="NpmAuthenticateTask"/> that provides npm credentials to an <c>.npmrc</c>
    /// file in your repository for the scope of the build. This enables npm and other npm-based tasks
    /// (e.g. <c>npm install</c>) to authenticate with private registries.
    /// </summary>
    /// <remarks>
    /// The task appends credentials to the selected <c>.npmrc</c> during the job and restores the file during post-job cleanup.
    /// Do not use this task with the Azure Pipelines npm task, which handles npm authentication itself.
    /// </remarks>
    /// <param name="workingFile">The path to the <c>.npmrc</c> file that specifies the registries you want to work with. Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.</param>
    /// <param name="customEndpoints">Optional list of npm service connection names for registries outside this organization/collection.</param>
    /// <returns>An <see cref="NpmAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// Npm.Authenticate(".npmrc", ["myServiceConnection"])
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: npmAuthenticate@0
    ///   inputs:
    ///     workingFile: .npmrc
    ///     customEndpoint: myServiceConnection
    /// </code>
    /// </example>
    public NpmAuthenticateTask Authenticate(string workingFile, string[]? customEndpoints = null)
    {
        var task = new NpmAuthenticateTask(workingFile);

        if (customEndpoints is not null)
        {
            task = task with { CustomEndpoints = customEndpoints };
        }

        return task;
    }

    /// <summary>
    /// Creates an <see cref="NpmAuthenticateTask"/> that authenticates to an Azure Artifacts npm feed with an Azure DevOps service connection.
    /// </summary>
    /// <remarks>
    /// This represents the Azure Pipelines-only <c>azureDevOpsServiceConnection</c> input, whose official YAML alias is
    /// <c>workloadIdentityServiceConnection</c>. This authentication mode requires <paramref name="feedUrl"/> and is not compatible with
    /// <see cref="NpmAuthenticateTask.CustomEndpoints"/>.
    /// </remarks>
    /// <param name="workingFile">The path to the <c>.npmrc</c> file that specifies the registries you want to work with. Select the file, not the folder.</param>
    /// <param name="azureDevOpsServiceConnection">The Azure DevOps service connection used for Workload Identity Federation authentication.</param>
    /// <param name="feedUrl">The Azure Artifacts feed URL in npm registry format.</param>
    /// <returns>An <see cref="NpmAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// Npm.Authenticate(".npmrc", "myAzureDevOpsServiceConnection", "https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/npm/registry/")
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: npmAuthenticate@0
    ///   inputs:
    ///     workingFile: .npmrc
    ///     azureDevOpsServiceConnection: myAzureDevOpsServiceConnection
    ///     feedUrl: https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/npm/registry/
    /// </code>
    /// </example>
    public NpmAuthenticateTask Authenticate(string workingFile, string azureDevOpsServiceConnection, string feedUrl)
    {
        if (string.IsNullOrWhiteSpace(azureDevOpsServiceConnection))
        {
            throw new System.ArgumentException($"'{nameof(azureDevOpsServiceConnection)}' cannot be null, empty, or whitespace.", nameof(azureDevOpsServiceConnection));
        }

        if (string.IsNullOrWhiteSpace(feedUrl))
        {
            throw new System.ArgumentException($"'{nameof(feedUrl)}' cannot be null, empty, or whitespace.", nameof(feedUrl));
        }

        return new(workingFile)
        {
            AzureDevOpsServiceConnection = azureDevOpsServiceConnection,
            FeedUrl = feedUrl
        };
    }
}
