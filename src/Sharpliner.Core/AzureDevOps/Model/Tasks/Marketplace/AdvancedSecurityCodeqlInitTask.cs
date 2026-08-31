using System;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/advanced-security-codeql-init-v1">AdvancedSecurity-Codeql-Init@1</see>
/// task in Azure DevOps pipelines.
/// </summary>
/// <remarks>
/// Audited against the current Microsoft Learn YAML task reference:
/// <see href="https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/advanced-security-codeql-init-v1.md">advanced-security-codeql-init-v1.md</see>
/// (<c>ms.date: 07/28/2026</c>).
/// </remarks>
public record AdvancedSecurityCodeqlInitTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedSecurityCodeqlInitTask"/> class.
    /// </summary>
    public AdvancedSecurityCodeqlInitTask() : base("AdvancedSecurity-Codeql-Init@1")
    {
        DisplayName = "Advanced Security Initialize CodeQL";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedSecurityCodeqlInitTask"/> class with one or more languages to analyze.
    /// </summary>
    /// <param name="languages">One or more languages to analyze.</param>
    /// <exception cref="ArgumentException">Thrown when no languages are provided.</exception>
    public AdvancedSecurityCodeqlInitTask(params CodeqlLanguage[] languages) : this()
    {
        Languages = ToCommaSeparatedValue(languages);
    }

    /// <summary>
    /// Gets or sets whether the task should automatically detect and install CodeQL when needed.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? EnableAutomaticCodeQLInstall
    {
        get => GetExpression<bool>("enableAutomaticCodeQLInstall");
        init => SetProperty("enableAutomaticCodeQLInstall", value);
    }

    /// <summary>
    /// Gets or sets whether previous task-managed CodeQL installs should be removed from the agent tool cache.
    /// Default value: <c>false</c>. Only applies when <see cref="EnableAutomaticCodeQLInstall"/> is set to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CleanupOldAutomaticInstalls
    {
        get => GetExpression<bool>("cleanupOldAutomaticInstalls");
        init => SetProperty("cleanupOldAutomaticInstalls", value);
    }

    /// <summary>
    /// Gets or sets the languages to analyze as a comma-separated list.
    /// Allowed values: <c>csharp</c>, <c>cpp</c>, <c>go</c>, <c>java</c>, <c>javascript</c>, <c>python</c>, <c>ruby</c>, <c>rust</c>, <c>swift</c>.
    /// </summary>
    /// <remarks>
    /// This input has no alias in the current official task reference.
    /// You can also provide the language through the <c>advancedsecurity.codeql.language</c> pipeline variable.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? Languages
    {
        get => GetExpression<string>("languages");
        init => SetProperty("languages", value);
    }

    /// <summary>
    /// Gets or sets the query suite used for analysis.
    /// Allowed values: <see cref="CodeqlQuerySuite.SelectAQuerySuite"/>, <see cref="CodeqlQuerySuite.CodeScanning"/>,
    /// <see cref="CodeqlQuerySuite.SecurityExtended"/>, <see cref="CodeqlQuerySuite.SecurityExperimental"/>,
    /// <see cref="CodeqlQuerySuite.SecurityAndQuality"/>.
    /// Default value: <see cref="CodeqlQuerySuite.SelectAQuerySuite"/>.
    /// </summary>
    [YamlIgnore]
    public CodeqlQuerySuite QuerySuite
    {
        get => GetEnum("querysuite", CodeqlQuerySuite.SelectAQuerySuite);
        init => SetProperty("querysuite", value);
    }

    /// <summary>
    /// Gets or sets the CodeQL build mode.
    /// Allowed values: <see cref="CodeqlBuildType.Manual"/>, <see cref="CodeqlBuildType.None"/>.
    /// Default value: <see cref="CodeqlBuildType.Manual"/>.
    /// </summary>
    [YamlIgnore]
    public CodeqlBuildType BuildType
    {
        get => GetEnum("buildtype", CodeqlBuildType.Manual);
        init => SetProperty("buildtype", value);
    }

    /// <summary>
    /// Gets or sets total RAM (in MB) available to CodeQL query evaluation.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Ram
    {
        get => GetExpression<string>("ram");
        init => SetProperty("ram", value);
    }

    /// <summary>
    /// Gets or sets the number of threads used to evaluate queries.
    /// You can pass <c>0</c> for one thread per core, or a negative value such as <c>-1</c> to leave cores unused.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Threads
    {
        get => GetExpression<string>("threads");
        init => SetProperty("threads", value);
    }

    /// <summary>
    /// Gets or sets a comma-separated list of paths to exclude from analysis.
    /// Paths are relative to <see cref="SourcesFolder"/> (or to <c>Build.SourcesDirectory</c> when not set).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CodeqlPathsToIgnore
    {
        get => GetExpression<string>("codeqlpathstoignore");
        init => SetProperty("codeqlpathstoignore", value);
    }

    /// <summary>
    /// Gets or sets a comma-separated list of additional paths to include in analysis.
    /// Paths are relative to <see cref="SourcesFolder"/> (or to <c>Build.SourcesDirectory</c> when not set).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CodeqlPathsToInclude
    {
        get => GetExpression<string>("codeqlpathstoinclude");
        init => SetProperty("codeqlpathstoinclude", value);
    }

    /// <summary>
    /// Gets or sets the folder that contains the sources to analyze.
    /// The value should be relative to <c>Build.SourcesDirectory</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SourcesFolder
    {
        get => GetExpression<string>("sourcesfolder");
        init => SetProperty("sourcesfolder", value);
    }

    /// <summary>
    /// Gets or sets the log level for analysis.
    /// Allowed values: <see cref="CodeqlLogLevel.Warning"/>, <see cref="CodeqlLogLevel.Verbose"/>,
    /// <see cref="CodeqlLogLevel.Debug"/>, <see cref="CodeqlLogLevel.DefaultWarning"/>.
    /// Default value: <see cref="CodeqlLogLevel.DefaultWarning"/>.
    /// </summary>
    [YamlIgnore]
    public CodeqlLogLevel LogLevel
    {
        get => GetEnum("loglevel", CodeqlLogLevel.DefaultWarning);
        init => SetProperty("loglevel", value);
    }

    /// <summary>
    /// Gets or sets the absolute path to a custom CodeQL configuration file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConfigFilePath
    {
        get => GetExpression<string>("configfilepath");
        init => SetProperty("configfilepath", value);
    }

    /// <summary>
    /// Gets or sets the absolute path to a custom CodeQL tools directory.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CodeqlToolsDirectory
    {
        get => GetExpression<string>("codeqltoolsdirectory");
        init => SetProperty("codeqltoolsdirectory", value);
    }

    /// <summary>
    /// Sets languages to analyze using strongly typed language values.
    /// </summary>
    /// <param name="languages">One or more languages to analyze.</param>
    /// <returns>The current task instance with updated languages.</returns>
    /// <exception cref="ArgumentException">Thrown when no languages are provided.</exception>
    public AdvancedSecurityCodeqlInitTask WithLanguages(params CodeqlLanguage[] languages)
        => this with
        {
            Languages = ToCommaSeparatedValue(languages),
        };

    private static string ToCommaSeparatedValue(params CodeqlLanguage[] languages)
    {
        if (languages.Length == 0)
        {
            throw new ArgumentException("At least one language must be specified.", nameof(languages));
        }

        return string.Join(",", languages.Select(language => YamlStringEnumConverter.GetEnumValue(typeof(CodeqlLanguage), language)));
    }
}

/// <summary>
/// Allowed languages for <see cref="AdvancedSecurityCodeqlInitTask"/> <c>languages</c> input.
/// </summary>
public enum CodeqlLanguage
{
    /// <summary>
    /// C#.
    /// </summary>
    [YamlMember(Alias = "csharp")]
    CSharp,

    /// <summary>
    /// C/C++.
    /// </summary>
    [YamlMember(Alias = "cpp")]
    Cpp,

    /// <summary>
    /// Go.
    /// </summary>
    [YamlMember(Alias = "go")]
    Go,

    /// <summary>
    /// Java.
    /// </summary>
    [YamlMember(Alias = "java")]
    Java,

    /// <summary>
    /// JavaScript.
    /// </summary>
    [YamlMember(Alias = "javascript")]
    JavaScript,

    /// <summary>
    /// Python.
    /// </summary>
    [YamlMember(Alias = "python")]
    Python,

    /// <summary>
    /// Ruby.
    /// </summary>
    [YamlMember(Alias = "ruby")]
    Ruby,

    /// <summary>
    /// Rust.
    /// </summary>
    [YamlMember(Alias = "rust")]
    Rust,

    /// <summary>
    /// Swift.
    /// </summary>
    [YamlMember(Alias = "swift")]
    Swift,
}

/// <summary>
/// CodeQL query suite values for <see cref="AdvancedSecurityCodeqlInitTask"/>.
/// </summary>
public enum CodeqlQuerySuite
{
    /// <summary>
    /// Placeholder option indicating query suite selection should come from pipeline variable configuration.
    /// </summary>
    [YamlMember(Alias = "Select a query suite...")]
    SelectAQuerySuite,

    /// <summary>
    /// Standard code scanning suite.
    /// </summary>
    [YamlMember(Alias = "code-scanning")]
    CodeScanning,

    /// <summary>
    /// Extended security suite.
    /// </summary>
    [YamlMember(Alias = "security-extended")]
    SecurityExtended,

    /// <summary>
    /// Experimental security suite.
    /// </summary>
    [YamlMember(Alias = "security-experimental")]
    SecurityExperimental,

    /// <summary>
    /// Security and quality suite.
    /// </summary>
    [YamlMember(Alias = "security-and-quality")]
    SecurityAndQuality,
}

/// <summary>
/// CodeQL initialization build modes.
/// </summary>
public enum CodeqlBuildType
{
    /// <summary>
    /// Manual build mode.
    /// </summary>
    [YamlMember(Alias = "Manual")]
    Manual,

    /// <summary>
    /// No-build mode.
    /// </summary>
    [YamlMember(Alias = "None")]
    None,
}

/// <summary>
/// CodeQL log level values for <see cref="AdvancedSecurityCodeqlInitTask"/>.
/// </summary>
public enum CodeqlLogLevel
{
    /// <summary>
    /// Warning.
    /// </summary>
    [YamlMember(Alias = "0")]
    Warning,

    /// <summary>
    /// Verbose.
    /// </summary>
    [YamlMember(Alias = "1")]
    Verbose,

    /// <summary>
    /// Debug.
    /// </summary>
    [YamlMember(Alias = "2")]
    Debug,

    /// <summary>
    /// Default warning-level behavior.
    /// </summary>
    [YamlMember(Alias = "_")]
    DefaultWarning,
}
