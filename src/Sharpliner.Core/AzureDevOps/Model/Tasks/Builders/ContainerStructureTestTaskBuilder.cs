using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>ContainerStructureTest@0</c> tasks.
/// </summary>
public class ContainerStructureTestTaskBuilder
{
    internal ContainerStructureTestTaskBuilder()
    {
    }

    /// <summary>
    /// <para>
    /// Creates a task that validates a container image with <c>ContainerStructureTest@0</c>.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     ContainerStructureTest.Run("my-docker-connection", "my-org/my-image", "tests/container-structure.yaml") with
    ///     {
    ///         Tag = "1.0.0",
    ///         FailTaskOnFailedTests = true,
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: ContainerStructureTest@0
    ///   inputs:
    ///     dockerRegistryServiceConnection: my-docker-connection
    ///     repository: my-org/my-image
    ///     configFile: tests/container-structure.yaml
    ///     tag: 1.0.0
    ///     failTaskOnFailedTests: true
    /// </code>
    /// </summary>
    /// <param name="dockerRegistryServiceConnection">Docker registry service connection used to authenticate and pull the image.</param>
    /// <param name="repository">Container repository name.</param>
    /// <param name="configFile">Path to a <c>.yaml</c> or <c>.json</c> container-structure-test configuration file.</param>
    /// <param name="tag">Optional image tag. Defaults to <c>$(Build.BuildId)</c>.</param>
    public ContainerStructureTestTask Run(
        AdoExpression<string> dockerRegistryServiceConnection,
        AdoExpression<string> repository,
        AdoExpression<string> configFile,
        AdoExpression<string>? tag = null)
    {
        return new ContainerStructureTestTask(dockerRegistryServiceConnection, repository, configFile)
        {
            Tag = tag,
        };
    }
}
