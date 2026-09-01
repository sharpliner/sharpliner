using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Write service image digests</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeWriteImageDigestsTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeWriteImageDigestsTask"/> class.
    /// </summary>
    public DockerComposeWriteImageDigestsTask() : base("Write service image digests")
    {
        DisplayName = "Docker Compose write service image digests";
    }

    /// <summary>
    /// Output Docker Compose file path that will contain pinned image digests.
    /// DockerCompose@1 requires this input for this action and defaults it to
    /// <c>$(Build.StagingDirectory)/docker-compose.images.yml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ImageDigestComposeFile
    {
        get => GetExpression<string>("imageDigestComposeFile");
        init => SetProperty("imageDigestComposeFile", value);
    }
}
