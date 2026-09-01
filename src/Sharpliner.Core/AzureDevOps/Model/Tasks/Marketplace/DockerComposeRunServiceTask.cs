using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Run a specific service</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeRunServiceTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeRunServiceTask"/> class.
    /// </summary>
    /// <param name="serviceName">Required Docker Compose service name.</param>
    public DockerComposeRunServiceTask(string serviceName) : base("Run a specific service")
    {
        DisplayName = "Docker Compose run a specific service";
        SetProperty("serviceName", Require.NotNullAndNotEmpty(serviceName));
    }

    /// <summary>
    /// Required service name to run.
    /// This corresponds to DockerCompose@1 input <c>serviceName</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ServiceName => GetExpression<string>("serviceName")!;

    /// <summary>
    /// Optional container name override.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ContainerName
    {
        get => GetExpression<string>("containerName");
        init => SetProperty("containerName", value);
    }

    /// <summary>
    /// Optional port mappings for the service.
    /// Provide multiple values as a new-line separated <c>multiLine</c> input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Ports
    {
        get => GetExpression<string>("ports");
        init => SetProperty("ports", value);
    }

    /// <summary>
    /// Optional working directory passed to the container.
    /// The official DockerCompose@1 input is <c>workDir</c>; <c>workingDirectory</c> is its documented alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workDir");
        init => SetProperty("workDir", value);
    }

    /// <summary>
    /// Optional container entry point override.
    /// This corresponds to DockerCompose@1 input <c>entrypoint</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? EntryPoint
    {
        get => GetExpression<string>("entrypoint");
        init => SetProperty("entrypoint", value);
    }

    /// <summary>
    /// Optional command passed to the container.
    /// This corresponds to DockerCompose@1 input <c>containerCommand</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ContainerCommand
    {
        get => GetExpression<string>("containerCommand");
        init => SetProperty("containerCommand", value);
    }

    /// <summary>
    /// Optional flag that runs the service in the background.
    /// Default: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Detached
    {
        get => GetExpression<bool>("detached");
        init => SetProperty("detached", value);
    }
}
