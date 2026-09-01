using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/cmake-v1">CMake@1</see>
/// task in Azure DevOps pipelines, which builds projects with the CMake cross-platform build system.
/// More details can be found in the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/CMakeV1/task.json">official CMakeV1 task specification</see>.
/// </summary>
/// <remarks>
/// This model was audited against task version <c>1.279.0</c> on 2026-08-31.
/// The task requires the <c>cmake</c> agent capability and agent version 1.91.0 or later. Microsoft-hosted agents
/// already provide CMake. All inputs are optional, and the task has no output variables.
/// </remarks>
public record CMakeTask : AzureDevOpsTask
{
    /// <summary>
    /// Gets or sets the current working directory in which CMake runs.
    /// </summary>
    /// <remarks>
    /// This is the optional <c>filePath</c> input named <c>cwd</c>, which also accepts the <c>workingDirectory</c> alias.
    /// The default is <c>build</c>. Relative paths are resolved from the repository, and CMake creates the directory
    /// if it does not exist. Full paths and Azure Pipelines variables are also supported.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("cwd", "build");
        init => SetProperty("cwd", value);
    }

    /// <summary>
    /// Gets or sets the arguments passed to CMake, such as a source directory, generator, definitions, or build options.
    /// </summary>
    /// <remarks>
    /// The optional <c>string</c> input named <c>cmakeArgs</c> defaults to an empty string.
    /// See the <see href="https://cmake.org/cmake/help/latest/manual/cmake.1.html">official CMake command-line reference</see>
    /// for supported arguments.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("cmakeArgs", string.Empty);
        init => SetProperty("cmakeArgs", value);
    }

    /// <summary>
    /// Gets or sets whether the CMake command runs inside an operating-system-specific shell.
    /// </summary>
    /// <remarks>
    /// The optional <c>boolean</c> input named <c>runInsideShell</c> defaults to <c>false</c>. Enable this when arguments
    /// need shell handling, such as expansion of environment variables. Shell interpretation is operating-system specific.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? RunInsideShell
    {
        get => GetExpression<bool>("runInsideShell", false);
        init => SetProperty("runInsideShell", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CMakeTask"/> class.
    /// </summary>
    public CMakeTask() : base("CMake@1")
    {
    }
}
