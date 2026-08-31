using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>Docker@2</c> Azure Pipelines task, which builds or pushes Docker images, logs in or out of a
/// registry, or starts/stops containers.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-v2">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerV2/task.json">official DockerV2 task specification</see>.
/// Use the <c>Docker</c> builder (e.g. <c>Docker.Build</c>, <c>Docker.Push</c>, <c>Docker.BuildAndPush</c>, <c>Docker.Login</c>,
/// <c>Docker.Logout</c>, <c>Docker.Start</c>, <c>Docker.Stop</c>) to create instances of this task's command-specific specializations.
/// </summary>
public record DockerTask : AzureDevOpsTask
{
    internal const string CommandProperty = "command";
    internal const string ContainerRegistryProperty = "containerRegistry";
    internal const string AddPipelineDataProperty = "addPipelineData";
    internal const string AddBaseImageDataProperty = "addBaseImageData";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerTask"/> class with the specified command.
    /// </summary>
    /// <param name="command">The Docker command (e.g. <c>build</c>, <c>push</c>, <c>buildAndPush</c>, <c>login</c>, <c>logout</c>, <c>start</c>, <c>stop</c>)</param>
    public DockerTask(string command) : base("Docker@2")
    {
        SetProperty(CommandProperty, command);
    }

    /// <summary>
    /// Docker registry service connection to authenticate with. Required for commands that need to authenticate with
    /// a registry (<c>login</c>, <c>logout</c>, <c>push</c>, <c>buildAndPush</c>).
    /// Docker@2 input: <c>containerRegistry</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ContainerRegistry
    {
        get => GetExpression<string>(ContainerRegistryProperty);
        init => SetProperty(ContainerRegistryProperty, value);
    }

    /// <summary>
    /// By default, pipeline metadata such as the source branch name and build ID are added to the built image(s), which
    /// helps with traceability. Set this to <c>false</c> to opt out.
    /// Docker@2 defaults this input to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? AddPipelineData
    {
        get => GetExpression<bool>(AddPipelineDataProperty);
        init => SetProperty(AddPipelineDataProperty, value);
    }

    /// <summary>
    /// By default, base image metadata such as the base image name and digest are added to the built image(s), which
    /// helps with traceability. Set this to <c>false</c> to opt out.
    /// Docker@2 defaults this input to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? AddBaseImageData
    {
        get => GetExpression<bool>(AddBaseImageDataProperty);
        init => SetProperty(AddBaseImageDataProperty, value);
    }
}
