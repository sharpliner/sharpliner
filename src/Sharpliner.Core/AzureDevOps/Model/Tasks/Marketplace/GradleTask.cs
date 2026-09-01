using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs Gradle using the built-in <c>Gradle@4</c> task.
/// More details can be found in the
/// <see href="https://docs.microsoft.com/azure/devops/pipelines/tasks/build/gradle">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/GradleV4/task.json">official GradleV4 task specification audited on 2026-08-31</see>.
/// </summary>
public record GradleTask : AzureDevOpsTask
{
    /// <summary>
    /// Required <c>filePath</c> input. Relative path from the repository root to the Gradle Wrapper script.
    /// Default value: <c>gradlew</c>.
    /// The official input name is <c>wrapperScript</c>; <c>gradleWrapperFile</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? GradleWrapperFile
    {
        get => GetExpression<string>("gradleWrapperFile", "gradlew");
        init => SetProperty("gradleWrapperFile", value);
    }

    /// <summary>
    /// Optional <c>filePath</c> input. Working directory in which to run the Gradle build.
    /// Empty means the repository root.
    /// The official input name is <c>cwd</c>; <c>workingDirectory</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. Additional Gradle command-line options.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Options
    {
        get => GetExpression<string>("options");
        init => SetProperty("options", value);
    }

    /// <summary>
    /// Required <c>string</c> input. Space-separated Gradle tasks to run.
    /// Default value: <c>build</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Tasks
    {
        get => GetExpression<string>("tasks", "build");
        init => SetProperty("tasks", value);
    }

    /// <summary>
    /// Required <c>boolean</c> input in the <c>junitTestResults</c> group. Publishes JUnit test results produced by the Gradle build to Azure Pipelines.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishJUnitResults
    {
        get => GetExpression<bool>("publishJUnitResults", true);
        init => SetProperty("publishJUnitResults", value);
    }

    /// <summary>
    /// Required <c>filePath</c> input in the <c>junitTestResults</c> group.
    /// Only used when <see cref="PublishJUnitResults"/> is <c>true</c>.
    /// Default value: <c>**/TEST-*.xml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestResultsFiles
    {
        get => GetExpression<string>("testResultsFiles", "**/TEST-*.xml");
        init => SetProperty("testResultsFiles", value);
    }

    /// <summary>
    /// Optional <c>string</c> input in the <c>junitTestResults</c> group.
    /// Only used when <see cref="PublishJUnitResults"/> is <c>true</c>.
    /// Provides a custom name for the Azure Pipelines test run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Required <c>radio</c> input in the <c>advanced</c> group.
    /// Sets <c>JAVA_HOME</c> either by selecting a discovered JDK version or by specifying a path manually.
    /// Default value: <see cref="global::Sharpliner.AzureDevOps.Tasks.JavaHomeSelection.JdkVersion"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<JavaHomeSelection>? JavaHomeSelection
    {
        get => GetExpression<JavaHomeSelection>("javaHomeSelection", global::Sharpliner.AzureDevOps.Tasks.JavaHomeSelection.JdkVersion);
        init => SetProperty("javaHomeSelection", value);
    }

    /// <summary>
    /// Optional <c>pickList</c> input in the <c>advanced</c> group.
    /// Only used when <see cref="JavaHomeSelection"/> is <see cref="global::Sharpliner.AzureDevOps.Tasks.JavaHomeSelection.JdkVersion"/>.
    /// Allowed values: <c>default</c>, <c>1.17</c>, <c>1.11</c>, <c>1.10</c>, <c>1.9</c>, <c>1.8</c>, <c>1.7</c>, and <c>1.6</c>.
    /// Default value: <c>default</c>.
    /// The official input name is <c>jdkVersion</c>; <c>jdkVersionOption</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JdkVersion
    {
        get => GetExpression<string>("jdkVersion", "default");
        init => SetProperty("jdkVersion", value);
    }

    /// <summary>
    /// Required <c>string</c> input in the <c>advanced</c> group when visible.
    /// Only used when <see cref="JavaHomeSelection"/> is <see cref="global::Sharpliner.AzureDevOps.Tasks.JavaHomeSelection.Path"/>.
    /// Sets <c>JAVA_HOME</c> to the supplied JDK path.
    /// The official input name is <c>jdkUserInputPath</c>; <c>jdkDirectory</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JdkUserInputPath
    {
        get => GetExpression<string>("jdkUserInputPath");
        init => SetProperty("jdkUserInputPath", value);
    }

    /// <summary>
    /// Optional <c>pickList</c> input in the <c>advanced</c> group.
    /// Only used when <see cref="JdkVersion"/> is not <c>default</c>.
    /// Default value: <see cref="global::Sharpliner.AzureDevOps.Tasks.JdkArchitecture.X64"/>.
    /// The official input name is <c>jdkArchitecture</c>; <c>jdkArchitectureOption</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<JdkArchitecture>? JdkArchitecture
    {
        get => GetExpression<JdkArchitecture>("jdkArchitecture", global::Sharpliner.AzureDevOps.Tasks.JdkArchitecture.X64);
        init => SetProperty("jdkArchitecture", value);
    }

    /// <summary>
    /// Optional <c>string</c> input in the <c>advanced</c> group. Sets the <c>GRADLE_OPTS</c> environment variable.
    /// Default value: <c>-Xmx1024m</c>.
    /// The official input name is <c>gradleOpts</c>; <c>gradleOptions</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? GradleOptions
    {
        get => GetExpression<string>("gradleOptions", "-Xmx1024m");
        init => SetProperty("gradleOptions", value);
    }

    /// <summary>
    /// Required <c>boolean</c> input in the <c>CodeAnalysis</c> group.
    /// Runs SonarQube or SonarCloud analysis after the Gradle tasks execute.
    /// Default value: <c>false</c>.
    /// The official input name is <c>sqAnalysisEnabled</c>; <c>sonarQubeRunAnalysis</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SonarQubeRunAnalysis
    {
        get => GetExpression<bool>("sonarQubeRunAnalysis", false);
        init => SetProperty("sonarQubeRunAnalysis", value);
    }

    /// <summary>
    /// Required <c>radio</c> input in the <c>CodeAnalysis</c> group.
    /// Only used when <see cref="SonarQubeRunAnalysis"/> is <c>true</c>.
    /// Default value: <see cref="global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<GradlePluginVersionChoice>? SonarQubeGradlePluginVersionChoice
    {
        get => GetExpression<GradlePluginVersionChoice>("sqGradlePluginVersionChoice", global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify);
        init => SetProperty("sqGradlePluginVersionChoice", value);
    }

    /// <summary>
    /// Required <c>string</c> input in the <c>CodeAnalysis</c> group when visible.
    /// Only used when <see cref="SonarQubeRunAnalysis"/> is <c>true</c> and <see cref="SonarQubeGradlePluginVersionChoice"/> is <see cref="global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify"/>.
    /// Default value: <c>2.6.1</c>.
    /// The official input name is <c>sqGradlePluginVersion</c>; <c>sonarQubeGradlePluginVersion</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeGradlePluginVersion
    {
        get => GetExpression<string>("sonarQubeGradlePluginVersion", "2.6.1");
        init => SetProperty("sonarQubeGradlePluginVersion", value);
    }

    /// <summary>
    /// Optional <c>boolean</c> input in the <c>CodeAnalysis</c> group.
    /// Runs Checkstyle with the default Sun checks and uploads the results as build artifacts.
    /// Default value: <c>false</c>.
    /// The official input name is <c>checkstyleAnalysisEnabled</c>; <c>checkStyleRunAnalysis</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckstyleAnalysisEnabled
    {
        get => GetExpression<bool>("checkstyleAnalysisEnabled", false);
        init => SetProperty("checkstyleAnalysisEnabled", value);
    }

    /// <summary>
    /// Optional <c>boolean</c> input in the <c>CodeAnalysis</c> group.
    /// Runs FindBugs and uploads the results as build artifacts.
    /// Default value: <c>false</c>.
    /// This plugin was removed in Gradle 6.0; prefer <see cref="SpotBugsAnalysisEnabled"/> for newer builds.
    /// The official input name is <c>findbugsAnalysisEnabled</c>; <c>findBugsRunAnalysis</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FindBugsAnalysisEnabled
    {
        get => GetExpression<bool>("findbugsAnalysisEnabled", false);
        init => SetProperty("findbugsAnalysisEnabled", value);
    }

    /// <summary>
    /// Optional <c>boolean</c> input in the <c>CodeAnalysis</c> group.
    /// Runs PMD and uploads the results as build artifacts.
    /// Default value: <c>false</c>.
    /// The official input name is <c>pmdAnalysisEnabled</c>; <c>pmdRunAnalysis</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PmdAnalysisEnabled
    {
        get => GetExpression<bool>("pmdAnalysisEnabled", false);
        init => SetProperty("pmdAnalysisEnabled", value);
    }

    /// <summary>
    /// Required <c>boolean</c> input in the <c>CodeAnalysis</c> group.
    /// Runs SpotBugs. This plugin works with Gradle 5.6 or later.
    /// Default value: <c>false</c>.
    /// The official input name is <c>spotBugsAnalysisEnabled</c>; <c>spotBugsAnalysis</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SpotBugsAnalysisEnabled
    {
        get => GetExpression<bool>("spotBugsAnalysisEnabled", false);
        init => SetProperty("spotBugsAnalysisEnabled", value);
    }

    /// <summary>
    /// Required <c>radio</c> input in the <c>CodeAnalysis</c> group.
    /// Only used when <see cref="SpotBugsAnalysisEnabled"/> is <c>true</c>.
    /// Default value: <see cref="global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<GradlePluginVersionChoice>? SpotBugsGradlePluginVersionChoice
    {
        get => GetExpression<GradlePluginVersionChoice>("spotBugsGradlePluginVersionChoice", global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify);
        init => SetProperty("spotBugsGradlePluginVersionChoice", value);
    }

    /// <summary>
    /// Required <c>string</c> input in the <c>CodeAnalysis</c> group when visible.
    /// Only used when <see cref="SpotBugsAnalysisEnabled"/> is <c>true</c> and <see cref="SpotBugsGradlePluginVersionChoice"/> is <see cref="global::Sharpliner.AzureDevOps.Tasks.GradlePluginVersionChoice.Specify"/>.
    /// Default value: <c>4.7.0</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SpotBugsGradlePluginVersion
    {
        get => GetExpression<string>("spotbugsGradlePluginVersion", "4.7.0");
        init => SetProperty("spotbugsGradlePluginVersion", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GradleTask"/> class with required properties.
    /// </summary>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    public GradleTask(AdoExpression<string>? wrapperScript = null, AdoExpression<string>? tasks = null)
        : base("Gradle@4")
    {
        GradleWrapperFile = wrapperScript ?? "gradlew";
        Tasks = tasks ?? "build";
    }
}

/// <summary>
/// Supported values for the <c>javaHomeSelection</c> input of <see cref="GradleTask"/>.
/// </summary>
public enum JavaHomeSelection
{
    /// <summary>
    /// Discover and use a specific JDK version on the agent.
    /// </summary>
    [YamlMember(Alias = "JDKVersion")]
    JdkVersion,

    /// <summary>
    /// Set <c>JAVA_HOME</c> from a manually supplied path.
    /// </summary>
    [YamlMember(Alias = "Path")]
    Path,
}

/// <summary>
/// Supported values for the <c>jdkArchitecture</c> input of <see cref="GradleTask"/>.
/// </summary>
public enum JdkArchitecture
{
    /// <summary>
    /// 32-bit x86 JDK.
    /// </summary>
    [YamlMember(Alias = "x86")]
    X86,

    /// <summary>
    /// 64-bit x64 JDK.
    /// </summary>
    [YamlMember(Alias = "x64")]
    X64,

    /// <summary>
    /// 64-bit ARM JDK.
    /// </summary>
    [YamlMember(Alias = "arm64")]
    Arm64,
}

/// <summary>
/// Supported values for the Gradle plugin version choice inputs of <see cref="GradleTask"/>.
/// </summary>
public enum GradlePluginVersionChoice
{
    /// <summary>
    /// Specify the plugin version directly in the task input.
    /// </summary>
    [YamlMember(Alias = "specify")]
    Specify,

    /// <summary>
    /// Use the plugin version declared in <c>build.gradle</c>.
    /// </summary>
    [YamlMember(Alias = "build")]
    Build,
}
