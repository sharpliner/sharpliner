using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Run a Docker Compose command</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeCommandTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeCommandTask"/> class.
    /// </summary>
    /// <param name="command">Required Docker Compose command, such as <c>config</c> or <c>pull</c>.</param>
    public DockerComposeCommandTask(string command) : base("Run a Docker Compose command")
    {
        DisplayName = "Docker Compose command";
        SetProperty("dockerComposeCommand", Require.NotNullAndNotEmpty(command));
    }

    /// <summary>
    /// Required Docker Compose command executed by the task.
    /// This corresponds to DockerCompose@1 input <c>dockerComposeCommand</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> DockerComposeCommand => GetExpression<string>("dockerComposeCommand")!;
}
