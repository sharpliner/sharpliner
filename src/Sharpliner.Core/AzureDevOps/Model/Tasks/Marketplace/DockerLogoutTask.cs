namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>logout</c> command of Docker@2, which removes authentication for a container registry.
/// When <see cref="DockerTask.ContainerRegistry"/> is not set, Docker@2 removes all authentication data from the
/// temporary Docker config instead.
/// </summary>
public record DockerLogoutTask : DockerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerLogoutTask"/> class.
    /// </summary>
    public DockerLogoutTask() : base("logout")
    {
        DisplayName = "docker logout";
    }
}
