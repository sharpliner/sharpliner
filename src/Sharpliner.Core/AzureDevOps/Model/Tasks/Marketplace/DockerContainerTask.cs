using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base for the Docker@2 commands that operate on an already built container (<c>start</c>, <c>stop</c>), which
/// share the <see cref="Container"/> and <see cref="Arguments"/> inputs.
/// </summary>
public abstract record DockerContainerTask : DockerTask
{
    internal const string ContainerProperty = "container";
    internal const string ArgumentsProperty = "arguments";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerContainerTask"/> class with the specified command.
    /// </summary>
    /// <param name="command">The Docker command</param>
    protected DockerContainerTask(string command) : base(command)
    {
    }

    /// <summary>
    /// Name of the container.
    /// Docker@2 input: <c>container</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Container
    {
        get => GetExpression<string>(ContainerProperty);
        init => SetProperty(ContainerProperty, value);
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

/// <summary>
/// Task represents the <c>start</c> command of Docker@2, which starts a container.
/// </summary>
public record DockerStartTask : DockerContainerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerStartTask"/> class.
    /// </summary>
    public DockerStartTask() : base("start")
    {
        DisplayName = "docker start";
    }
}

/// <summary>
/// Task represents the <c>stop</c> command of Docker@2, which stops a container.
/// </summary>
public record DockerStopTask : DockerContainerTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerStopTask"/> class.
    /// </summary>
    public DockerStopTask() : base("stop")
    {
        DisplayName = "docker stop";
    }
}
