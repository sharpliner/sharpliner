using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the NuGetCommand@2 task for executing custom NuGet commands in Azure DevOps pipelines.
/// </summary>
/// <example>
/// <code>
/// var customCommandTask = new NuGetCustomCommandTask
/// {
///     Arguments = "-arg1 value1 -arg2 value2"
/// };
/// </code>
/// 
/// The YAML representation of the above C# code:
/// 
/// <code>
/// - task: NuGetCommand@2
///   inputs:
///     command: custom
///     arguments: -arg1 value1 -arg2 value2
/// </code>
/// </example>
public record NuGetCustomCommandTask : NuGetCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetCustomCommandTask"/> class.
    /// </summary>
    public NuGetCustomCommandTask(string arguments) : base("custom")
    {
        Arguments = Require.NotNullAndNotEmpty(arguments);
    }

    /// <summary>
    /// Gets or sets the arguments for the custom command.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? Arguments
    {
        get => GetConditioned<string>("arguments");
        init => SetProperty("arguments", value);
    }
}
