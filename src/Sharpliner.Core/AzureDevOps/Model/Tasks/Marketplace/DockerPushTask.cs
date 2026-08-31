using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>push</c> command of Docker@2, which pushes an image to a container registry.
/// </summary>
public record DockerPushTask : DockerImageTask
{
    internal const string ArgumentsProperty = "arguments";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerPushTask"/> class.
    /// </summary>
    public DockerPushTask() : base("push")
    {
        DisplayName = "docker push";
    }

    /// <summary>
    /// Additional Docker command options.
    /// Docker@2 input: <c>arguments</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>(ArgumentsProperty);
        init => SetProperty(ArgumentsProperty, value);
    }
}
