using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Defines the <c>VSBuild@1</c> Azure Pipelines task (Visual Studio build).
/// More details can be found in the <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/build/visual-studio-build">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/VSBuildV1/task.json">official task specification</see>.
/// </summary>
public record VSBuildTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VSBuildTask"/> class with required properties.
    /// </summary>
    /// <param name="solution">Relative path from repo root of the solution(s) or MSBuild project to run. Wildcards are supported, for example <c>**\*.sln</c>.</param>
    public VSBuildTask(AdoExpression<string> solution) : base("VSBuild@1")
    {
        Solution = solution;
        DisplayName = "Visual Studio build";
    }

    /// <summary>
    /// Relative path from repo root of the solution(s) or MSBuild project to run.
    /// This input is required by <c>VSBuild@1</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Solution
    {
        get => GetExpression<string>("solution");
        init => SetProperty("solution", value);
    }

    /// <summary>
    /// Preferred Visual Studio version to use.
    /// If the preferred version is unavailable, the task falls back to the latest installed version.
    /// Default value: <c>latest</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSBuildVisualStudioVersion>? VsVersion
    {
        get => GetExpression<VSBuildVisualStudioVersion>("vsVersion");
        init => SetProperty("vsVersion", value);
    }

    /// <summary>
    /// Additional command-line arguments passed to MSBuild.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MSBuildArgs
    {
        get => GetExpression<string>("msbuildArgs");
        init => SetProperty("msbuildArgs", value);
    }

    /// <summary>
    /// Build platform, for example <c>Win32</c>, <c>x86</c>, <c>x64</c>, or <c>Any CPU</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Platform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Build configuration, for example <c>Debug</c> or <c>Release</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Configuration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// If true, performs a clean build.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Clean
    {
        get => GetExpression<bool>("clean");
        init => SetProperty("clean", value);
    }

    /// <summary>
    /// If true, passes the <c>/m</c> switch to MSBuild to build in parallel (Windows only).
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? MaximumCpuCount
    {
        get => GetExpression<bool>("maximumCpuCount");
        init => SetProperty("maximumCpuCount", value);
    }

    /// <summary>
    /// If true, restores NuGet packages as part of the build.
    /// This official task input is deprecated by Microsoft.
    /// Default value: <c>false</c>.
    /// </summary>
    [Obsolete("VSBuild@1 restoreNugetPackages is deprecated by Microsoft. Prefer an explicit NuGet restore task before VSBuild.")]
    [YamlIgnore]
    public AdoExpression<bool>? RestoreNugetPackages
    {
        get => GetExpression<bool>("restoreNugetPackages");
        init => SetProperty("restoreNugetPackages", value);
    }

    /// <summary>
    /// Architecture of MSBuild to run.
    /// Default value: <c>x86</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSBuildArchitecture>? MSBuildArchitecture
    {
        get => GetExpression<VSBuildArchitecture>("msbuildArchitecture");
        init => SetProperty("msbuildArchitecture", value);
    }

    /// <summary>
    /// If true, records timeline details for each project.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? LogProjectEvents
    {
        get => GetExpression<bool>("logProjectEvents");
        init => SetProperty("logProjectEvents", value);
    }

    /// <summary>
    /// If true, creates an MSBuild log file (Windows only).
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CreateLogFile
    {
        get => GetExpression<bool>("createLogFile");
        init => SetProperty("createLogFile", value);
    }

    /// <summary>
    /// Log file verbosity level.
    /// This input is used only when <see cref="CreateLogFile"/> is true.
    /// Default value: <c>normal</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSBuildLogFileVerbosity>? LogFileVerbosity
    {
        get => GetExpression<VSBuildLogFileVerbosity>("logFileVerbosity");
        init => SetProperty("logFileVerbosity", value);
    }

    /// <summary>
    /// If true, enables the default logger for MSBuild.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? EnableDefaultLogger
    {
        get => GetExpression<bool>("enableDefaultLogger");
        init => SetProperty("enableDefaultLogger", value);
    }

    /// <summary>
    /// Custom Visual Studio version string, for example <c>15.0</c>, <c>16.0</c>, or <c>17.0</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomVersion
    {
        get => GetExpression<string>("customVersion");
        init => SetProperty("customVersion", value);
    }
}

/// <summary>
/// Visual Studio versions supported by <see cref="VSBuildTask"/>.
/// </summary>
public enum VSBuildVisualStudioVersion
{
    /// <summary>
    /// Uses the latest installed Visual Studio version.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Uses Visual Studio 2026 (18.0).
    /// </summary>
    [YamlMember(Alias = "18.0")]
    VisualStudio2026,

    /// <summary>
    /// Uses Visual Studio 2022 (17.0).
    /// </summary>
    [YamlMember(Alias = "17.0")]
    VisualStudio2022,

    /// <summary>
    /// Uses Visual Studio 2019 (16.0).
    /// </summary>
    [YamlMember(Alias = "16.0")]
    VisualStudio2019,

    /// <summary>
    /// Uses Visual Studio 2017 (15.0).
    /// </summary>
    [YamlMember(Alias = "15.0")]
    VisualStudio2017,

    /// <summary>
    /// Uses Visual Studio 2015 (14.0).
    /// </summary>
    [YamlMember(Alias = "14.0")]
    VisualStudio2015,

    /// <summary>
    /// Uses Visual Studio 2013 (12.0).
    /// </summary>
    [YamlMember(Alias = "12.0")]
    VisualStudio2013,

    /// <summary>
    /// Uses Visual Studio 2012 (11.0).
    /// </summary>
    [YamlMember(Alias = "11.0")]
    VisualStudio2012,
}

/// <summary>
/// MSBuild architecture options supported by <see cref="VSBuildTask"/>.
/// </summary>
public enum VSBuildArchitecture
{
    /// <summary>
    /// Runs MSBuild x86.
    /// </summary>
    [YamlMember(Alias = "x86")]
    X86,

    /// <summary>
    /// Runs MSBuild x64.
    /// </summary>
    [YamlMember(Alias = "x64")]
    X64,

    /// <summary>
    /// Runs MSBuild arm64.
    /// </summary>
    [YamlMember(Alias = "arm64")]
    Arm64,
}

/// <summary>
/// Log file verbosity options for <see cref="VSBuildTask"/>.
/// </summary>
public enum VSBuildLogFileVerbosity
{
    /// <summary>
    /// Quiet verbosity.
    /// </summary>
    [YamlMember(Alias = "quiet")]
    Quiet,

    /// <summary>
    /// Minimal verbosity.
    /// </summary>
    [YamlMember(Alias = "minimal")]
    Minimal,

    /// <summary>
    /// Normal verbosity.
    /// </summary>
    [YamlMember(Alias = "normal")]
    Normal,

    /// <summary>
    /// Detailed verbosity.
    /// </summary>
    [YamlMember(Alias = "detailed")]
    Detailed,

    /// <summary>
    /// Diagnostic verbosity.
    /// </summary>
    [YamlMember(Alias = "diagnostic")]
    Diagnostic,
}
