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
    /// <param name="workingFile">The path to the <c>.npmrc</c> file that lists the registries you want to work with. Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.</param>
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
}
