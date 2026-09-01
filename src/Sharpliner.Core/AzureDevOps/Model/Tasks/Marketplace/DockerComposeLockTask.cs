using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Lock services</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeLockTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeLockTask"/> class.
    /// </summary>
    public DockerComposeLockTask() : base("Lock services")
    {
        DisplayName = "Docker Compose lock services";
    }

    /// <summary>
    /// Optional flag that removes build options from the resolved Docker Compose output.
    /// Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RemoveBuildOptions
    {
        get => GetExpression<bool>("removeBuildOptions");
        init => SetProperty("removeBuildOptions", value);
    }

    /// <summary>
    /// Optional base directory used to resolve relative references in the Docker Compose files.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BaseResolveDirectory
    {
        get => GetExpression<string>("baseResolveDirectory");
        init => SetProperty("baseResolveDirectory", value);
    }

    /// <summary>
    /// Output Docker Compose file path.
    /// DockerCompose@1 requires this input for the lock action and defaults it to
    /// <c>$(Build.StagingDirectory)/docker-compose.yml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OutputDockerComposeFile
    {
        get => GetExpression<string>("outputDockerComposeFile");
        init => SetProperty("outputDockerComposeFile", value);
    }
}
