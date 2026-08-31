using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops">official Azure DevOps pipelines documentation</see>
/// and here <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DotNetCoreCLIV2/task.json">AzDO task specification</see>.
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
    /// The path to the csproj or sln file(s) to use for build, restore, run, test, custom, and publish when
    /// <see cref="DotNetPublishCoreCliTask.PublishWebProjects"/> is false.
    /// You can use wildcards (e.g. <c>**/*.csproj</c> for all .csproj files in all subfolders).
    /// This field follows glob patterns against the root of the repository regardless of <see cref="WorkingDirectory"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Projects
    {
        get => GetExpression<string>("projects");
        init => SetProperty("projects", value);
    }

    /// <summary>
    /// Arguments to the selected command. For example, build configuration, output folder, runtime
    /// The arguments depend on the command selected.
    ///
    /// DotNetCoreCLI@2 accepts this input for build, publish, run, test, and custom commands.
    /// Restore uses <see cref="DotNetRestoreCoreCliTask.RestoreArguments"/> instead; pack and push do not support this input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// Current working directory where the script is run.
    /// Empty is the root of the repo (build) or artifacts (release), which is $(System.DefaultWorkingDirectory)
    /// This input is supported for build, publish, run, test, and custom commands.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Azure Resource Manager service connection. This input is named <c>ConnectedServiceName</c> in
    /// DotNetCoreCLI@2 and has the official alias <c>azureSubscription</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectedServiceName
    {
        get => GetExpression<string>("ConnectedServiceName");
        init => SetProperty("ConnectedServiceName", value);
    }

    /// <summary>
    /// Timeout in milliseconds for HTTP requests made by the task to obtain .NET package information.
    /// Defaults to 300000 milliseconds (5 minutes) in DotNetCoreCLI@2 and is capped by the task at 600000 milliseconds (10 minutes).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RequestTimeout
    {
        get => GetExpression<int>("requestTimeout");
        init => SetProperty("requestTimeout", value);
    }
}
