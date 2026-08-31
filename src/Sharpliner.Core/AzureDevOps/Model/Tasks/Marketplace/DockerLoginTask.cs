namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>login</c> command of Docker@2, which authenticates with a container registry.
/// Requires <see cref="DockerTask.ContainerRegistry"/> to be set.
/// </summary>
public record DockerLoginTask : DockerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerLoginTask"/> class.
    /// </summary>
    public DockerLoginTask() : base("login")
    {
        DisplayName = "docker login";
    }
}
