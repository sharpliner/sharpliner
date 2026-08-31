using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>build</c> command of Docker@2, which builds a Docker image from a Dockerfile.
/// </summary>
public record DockerBuildTask : DockerImageTask
{
    internal const string DockerfileProperty = "Dockerfile";
    internal const string BuildContextProperty = "buildContext";
    internal const string ArgumentsProperty = "arguments";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerBuildTask"/> class.
    /// </summary>
    public DockerBuildTask() : base("build")
    {
        DisplayName = "docker build";
    }

    /// <summary>
    /// Path to the Dockerfile.
    /// Docker@2 input: <c>Dockerfile</c>. Default: <c>**/Dockerfile</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Dockerfile
    {
        get => GetExpression<string>(DockerfileProperty);
        init => SetProperty(DockerfileProperty, value);
    }

    /// <summary>
    /// Path to the build context. Use <c>**</c> to specify the directory that contains the Dockerfile.
    /// Docker@2 input: <c>buildContext</c>. Default: <c>**</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildContext
    {
        get => GetExpression<string>(BuildContextProperty);
        init => SetProperty(BuildContextProperty, value);
    }

    /// <summary>
    /// Additional Docker command options, e.g. <c>--build-arg HTTP_PROXY=http://10.20.30.2:1234 --quiet</c>.
    /// Docker@2 input: <c>arguments</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>(ArgumentsProperty);
        init => SetProperty(ArgumentsProperty, value);
    }
}
