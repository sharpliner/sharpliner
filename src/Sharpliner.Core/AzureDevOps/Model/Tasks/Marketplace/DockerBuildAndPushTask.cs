using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>buildAndPush</c> command of Docker@2, which builds a Docker image and pushes it to a
/// container registry in one step. Docker@2 ignores <c>arguments</c> for this command.
/// </summary>
public record DockerBuildAndPushTask : DockerImageTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerBuildAndPushTask"/> class.
    /// </summary>
    public DockerBuildAndPushTask() : base("buildAndPush")
    {
        DisplayName = "docker build and push";
    }

    /// <summary>
    /// Path to the Dockerfile.
    /// Docker@2 input: <c>Dockerfile</c>. Default: <c>**/Dockerfile</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Dockerfile
    {
        get => GetExpression<string>(DockerBuildTask.DockerfileProperty);
        init => SetProperty(DockerBuildTask.DockerfileProperty, value);
    }

    /// <summary>
    /// Path to the build context. Use <c>**</c> to specify the directory that contains the Dockerfile.
    /// Docker@2 input: <c>buildContext</c>. Default: <c>**</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildContext
    {
        get => GetExpression<string>(DockerBuildTask.BuildContextProperty);
        init => SetProperty(DockerBuildTask.BuildContextProperty, value);
    }
}
