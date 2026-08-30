using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the abstract NuGetCommand@2 task in Azure DevOps pipelines.
/// Use this task to restore, pack, or push NuGet packages, or run a NuGet command.
/// This task supports NuGet.org and authenticated feeds like Azure Artifacts and MyGet.
/// This task also uses NuGet.exe and works with .NET Framework apps.
/// For .NET Core and .NET Standard apps, use the .NET Core task.
/// Modelled from the official Azure Pipelines
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/9dabcbcbcbc3b5a1a94fd32acaa2766fdf934bd6/Tasks/NuGetCommandV2/task.json">NuGetCommandV2 task.json</see>
/// task version 2.279.1.
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
    internal AdoExpression<string>? Command
    {
        get => GetExpression<string>("command");
        init => SetProperty("command", value);
    }
}

/// <summary>
/// Specifies the amount of detail displayed in NuGetCommand@2 output.
/// </summary>
public enum NuGetVerbosity
{
    /// <summary>
    /// Quiet verbosity.
    /// </summary>
    Quiet,

    /// <summary>
    /// Normal verbosity.
    /// </summary>
    Normal,

    /// <summary>
    /// Detailed verbosity.
    /// This is the default used by NuGetCommand@2 when verbosity is omitted.
    /// </summary>
    Detailed,
}
