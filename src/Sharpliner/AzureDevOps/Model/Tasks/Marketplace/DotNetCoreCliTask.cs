using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops">official Azure DevOps pipelines documentation</see>
/// and here <see href="https://github.com/microsoft/azure-pipelines-tasks/tree/master/Tasks/DotNetCoreCLIV2">AzDO task repository</see>.
/// </summary>
public record DotNetCoreCliTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetBuildCoreCliTask"/> class with the specified command.
    /// </summary>
    /// <param name="command">The dotnet command</param>
    public DotNetCoreCliTask(string command) : base("DotNetCoreCLI@2")
    {
        SetProperty("command", command);
    }

    /// <summary>
    /// The path to the csproj file(s) to use
    /// You can use wildcards (e.g. **/*.csproj for all .csproj files in all subfolders)
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? Projects
    {
        get => GetConditioned<string>("projects");
        init => SetProperty("projects", value);
    }

    /// <summary>
    /// Arguments to the selected command. For example, build configuration, output folder, runtime
    /// The arguments depend on the command selected.
    ///
    /// Note: This input only currently accepts arguments for build, publish, run, test, custom.
    /// If you would like to add arguments for a command not listed, use custom.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? Arguments
    {
        get => GetConditioned<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// Current working directory where the script is run.
    /// Empty is the root of the repo (build) or artifacts (release), which is $(System.DefaultWorkingDirectory)
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? WorkingDirectory
    {
        get => GetConditioned<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }
}
