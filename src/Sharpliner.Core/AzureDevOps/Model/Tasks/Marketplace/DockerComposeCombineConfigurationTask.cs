using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Combine configuration</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeCombineConfigurationTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeCombineConfigurationTask"/> class.
    /// </summary>
    public DockerComposeCombineConfigurationTask() : base("Combine configuration")
    {
        DisplayName = "Docker Compose combine configuration";
    }

    /// <summary>
    /// Optional flag that removes build options from the combined Docker Compose output.
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
    /// DockerCompose@1 requires this input for the combine action and defaults it to
    /// <c>$(Build.StagingDirectory)/docker-compose.yml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OutputDockerComposeFile
    {
        get => GetExpression<string>("outputDockerComposeFile");
        init => SetProperty("outputDockerComposeFile", value);
    }
}
