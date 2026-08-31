using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builds a project or solution with MSBuild using the <c>MSBuild@1</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/msbuild-v1">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/MSBuildV1/task.json">official MSBuildV1 task specification</see>.
/// </summary>
public record MSBuildTask : AzureDevOpsTask
{
    /// <summary>
    /// Required. Relative path from repo root of the project(s) or solution(s) to run. Wildcards can be used,
    /// for example <c>**/*.csproj</c> for all csproj files in all sub folders.
    /// Default value: <c>**/*.sln</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Solution
    {
        get => GetExpression<string>("solution");
        init => SetProperty("solution", value);
    }

    /// <summary>
    /// Optional. How MSBuild should be located: either by <see cref="Tasks.MSBuildLocationMethod.Version"/>
    /// or by a specific <see cref="Tasks.MSBuildLocationMethod.Location"/>.
    /// Default value: <see cref="Tasks.MSBuildLocationMethod.Version"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<MSBuildLocationMethod>? MSBuildLocationMethod
    {
        get => GetExpression<MSBuildLocationMethod>("msbuildLocationMethod");
        init => SetProperty("msbuildLocationMethod", value);
    }

    /// <summary>
    /// Optional. Used when <see cref="MSBuildLocationMethod"/> = <see cref="Tasks.MSBuildLocationMethod.Version"/>.
    /// Allowed values: <c>latest</c>, <c>18.0</c>, <c>17.0</c>, <c>16.0</c>, <c>15.0</c>, <c>14.0</c>, <c>12.0</c>, <c>4.0</c>.
    /// If the preferred version cannot be found, the latest version found will be used instead.
    /// On a macOS agent, xbuild (Mono) will be used if version is lower than 15.0.
    /// Default value: <c>latest</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MSBuildVersion
    {
        get => GetExpression<string>("msbuildVersion");
        init => SetProperty("msbuildVersion", value);
    }

    /// <summary>
    /// Optional. Used when <see cref="MSBuildLocationMethod"/> = <see cref="Tasks.MSBuildLocationMethod.Version"/>.
    /// Architecture of MSBuild to run.
    /// Default value: <see cref="Tasks.MSBuildArchitecture.X86"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<MSBuildArchitecture>? MSBuildArchitecture
    {
        get => GetExpression<MSBuildArchitecture>("msbuildArchitecture");
        init => SetProperty("msbuildArchitecture", value);
    }

    /// <summary>
    /// Optional. Used when <see cref="MSBuildLocationMethod"/> = <see cref="Tasks.MSBuildLocationMethod.Location"/>.
    /// Path to MSBuild.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MSBuildLocationPath
    {
        get => GetExpression<string>("msbuildLocation");
        init => SetProperty("msbuildLocation", value);
    }

    /// <summary>
    /// Optional. Platform to build, for example <c>x86</c>, <c>x64</c>, or <c>Any CPU</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Platform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Optional. Configuration to build, for example <c>debug</c> or <c>release</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Configuration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Optional. Additional arguments passed to MSBuild (on Windows) and xbuild (on macOS).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MSBuildArguments
    {
        get => GetExpression<string>("msbuildArguments");
        init => SetProperty("msbuildArguments", value);
    }

    /// <summary>
    /// Optional. Runs a clean build (<c>/t:clean</c>) prior to the build.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Clean
    {
        get => GetExpression<bool>("clean", false);
        init => SetProperty("clean", value);
    }

    /// <summary>
    /// Optional. If your MSBuild target configuration is compatible with building in parallel, you can optionally
    /// set this input to pass the <c>/m</c> switch to MSBuild (Windows only). If your target configuration is not
    /// compatible with building in parallel, enabling this option may cause your build to result in file-in-use
    /// errors, or intermittent or inconsistent build failures.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? MaximumCpuCount
    {
        get => GetExpression<bool>("maximumCpuCount", false);
        init => SetProperty("maximumCpuCount", value);
    }

    /// <summary>
    /// Optional. This option is deprecated. To restore NuGet packages, add a
    /// <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/package/nuget">NuGet</see> task before the build.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RestoreNugetPackages
    {
        get => GetExpression<bool>("restoreNugetPackages", false);
        init => SetProperty("restoreNugetPackages", value);
    }

    /// <summary>
    /// Optional. Records timeline details for each project (Windows only).
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? LogProjectEvents
    {
        get => GetExpression<bool>("logProjectEvents", false);
        init => SetProperty("logProjectEvents", value);
    }

    /// <summary>
    /// Optional. Creates a log file (Windows only).
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CreateLogFile
    {
        get => GetExpression<bool>("createLogFile", false);
        init => SetProperty("createLogFile", value);
    }

    /// <summary>
    /// Optional. Used when <see cref="CreateLogFile"/> = <c>true</c>. Log file verbosity.
    /// Default value: <see cref="MSBuildLogFileVerbosity.Normal"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<MSBuildLogFileVerbosity>? LogFileVerbosity
    {
        get => GetExpression<MSBuildLogFileVerbosity>("logFileVerbosity");
        init => SetProperty("logFileVerbosity", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MSBuildTask"/> class.
    /// </summary>
    /// <param name="solution">Relative path from repo root of the project(s) or solution(s) to run. Default value: <c>**/*.sln</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="solution"/> is null.</exception>
    public MSBuildTask(AdoExpression<string> solution)
        : base("MSBuild@1")
    {
        Solution = solution ?? throw new ArgumentNullException(nameof(solution));
    }
}

/// <summary>
/// Allowed values for MSBuild task <c>msbuildLocationMethod</c>.
/// </summary>
public enum MSBuildLocationMethod
{
    /// <summary>
    /// Default. Locate MSBuild by version.
    /// </summary>
    [YamlMember(Alias = "version")]
    Version,

    /// <summary>
    /// Specify the location of MSBuild explicitly.
    /// </summary>
    [YamlMember(Alias = "location")]
    Location,
}

/// <summary>
/// Allowed values for MSBuild task <c>msbuildArchitecture</c>.
/// </summary>
public enum MSBuildArchitecture
{
    /// <summary>
    /// Default. MSBuild x86.
    /// </summary>
    [YamlMember(Alias = "x86")]
    X86,

    /// <summary>
    /// MSBuild x64.
    /// </summary>
    [YamlMember(Alias = "x64")]
    X64,

    /// <summary>
    /// MSBuild arm64.
    /// </summary>
    [YamlMember(Alias = "arm64")]
    Arm64,
}

/// <summary>
/// Allowed values for MSBuild task <c>logFileVerbosity</c>.
/// </summary>
public enum MSBuildLogFileVerbosity
{
    /// <summary>
    /// Quiet.
    /// </summary>
    [YamlMember(Alias = "quiet")]
    Quiet,

    /// <summary>
    /// Minimal.
    /// </summary>
    [YamlMember(Alias = "minimal")]
    Minimal,

    /// <summary>
    /// Default. Normal.
    /// </summary>
    [YamlMember(Alias = "normal")]
    Normal,

    /// <summary>
    /// Detailed.
    /// </summary>
    [YamlMember(Alias = "detailed")]
    Detailed,

    /// <summary>
    /// Diagnostic.
    /// </summary>
    [YamlMember(Alias = "diagnostic")]
    Diagnostic,
}
