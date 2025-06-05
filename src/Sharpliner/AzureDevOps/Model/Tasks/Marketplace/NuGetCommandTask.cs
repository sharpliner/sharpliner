using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the abstract NuGetCommand@2 task in Azure DevOps pipelines.
/// Use this task to restore, pack, or push NuGet packages, or run a NuGet command.
/// This task supports NuGet.org and authenticated feeds like Azure Artifacts and MyGet.
/// This task also uses NuGet.exe and works with .NET Framework apps.
/// For .NET Core and .NET Standard apps, use the .NET Core task.
/// </summary>
public abstract record NuGetCommandTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetCommandTask"/> class.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    protected NuGetCommandTask(string command) : base("NuGetCommand@2")
    {
        Command = command;
    }

    [YamlIgnore]
    internal Conditioned<string>? Command
    {
        get => GetConditioned<string>("command");
        init => SetProperty("command", value);
    }
}
