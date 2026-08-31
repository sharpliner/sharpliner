using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> task with action <c>Build services</c>.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Docker Compose task reference</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">task specification</see>.
/// </summary>
public record DockerComposeBuildTask : DockerComposeTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeBuildTask"/> class.
    /// </summary>
    public DockerComposeBuildTask() : base("Build services")
    {
        DisplayName = "Docker Compose build services";
    }

    /// <summary>
    /// Optional additional tags to apply to built images.
    /// Provide multiple tags as a new-line separated <c>multiLine</c> value.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AdditionalImageTags
    {
        get => GetExpression<string>("additionalImageTags");
        init => SetProperty("additionalImageTags", value);
    }

    /// <summary>
    /// Optional flag that includes source tags on built images.
    /// Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeSourceTags
    {
        get => GetExpression<bool>("includeSourceTags");
        init => SetProperty("includeSourceTags", value);
    }

    /// <summary>
    /// Optional flag that includes the <c>latest</c> tag on built images.
    /// Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeLatestTag
    {
        get => GetExpression<bool>("includeLatestTag");
        init => SetProperty("includeLatestTag", value);
    }
}
