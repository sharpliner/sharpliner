namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create various npm tasks in Azure DevOps pipelines.
/// </summary>
public class NpmTaskBuilder
{
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
            throw new System.ArgumentException($"'{nameof(azureDevOpsServiceConnection)}' cannot be null or empty.", nameof(azureDevOpsServiceConnection));
        }

        if (string.IsNullOrWhiteSpace(feedUrl))
        {
            throw new System.ArgumentException($"'{nameof(feedUrl)}' cannot be null or empty.", nameof(feedUrl));
        }

        return new(workingFile)
        {
            AzureDevOpsServiceConnection = azureDevOpsServiceConnection,
            FeedUrl = feedUrl
        };
    }
}
