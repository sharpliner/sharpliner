using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Run services</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeRunTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRunTask"/> class.
    /// </summary>
    public DockerComposeRunTask() : base("Run services")
    {
        DisplayName = "Docker Compose run services";
    }

    /// <summary>
    /// Optional flag that builds images before running services.
    /// Default: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? BuildImages
    {
        get => GetExpression<bool>("buildImages");
        init => SetProperty("buildImages", value);
    }

    /// <summary>
    /// Optional flag that runs services in the background.
    /// Default: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Detached
    {
        get => GetExpression<bool>("detached");
        init => SetProperty("detached", value);
    }

    /// <summary>
    /// Optional flag that stops all containers when one container exits.
    /// DockerCompose@1 honors this input only when <see cref="Detached"/> is <c>false</c>.
    /// Default: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? AbortOnContainerExit
    {
        get => GetExpression<bool>("abortOnContainerExit");
        init => SetProperty("abortOnContainerExit", value);
    }
}
