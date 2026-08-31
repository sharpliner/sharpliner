using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Common API shared by the Azure DevOps <c>Maven</c> task family.
/// See the version-specific task types such as <see cref="MavenTask"/>, <see cref="MavenV3Task"/>, <see cref="MavenV2Task"/>,
/// and <see cref="MavenV1Task"/> for the corresponding Microsoft Learn reference pages and task specifications.
/// </summary>
public abstract record MavenTaskBase : AzureDevOpsTask
{
    private const string MavenPOMFileProperty = "mavenPOMFile";
    private const string GoalsProperty = "goals";
    private const string OptionsProperty = "options";
    private const string PublishJUnitResultsProperty = "publishJUnitResults";
    private const string TestResultsFilesProperty = "testResultsFiles";
    private const string TestRunTitleProperty = "testRunTitle";
    private const string CodeCoverageToolProperty = "codeCoverageTool";
    private const string ClassFilterProperty = "classFilter";
    private const string ClassFilesDirectoriesProperty = "classFilesDirectories";
    private const string SrcDirectoriesProperty = "srcDirectories";
    private const string FailIfCoverageEmptyProperty = "failIfCoverageEmpty";
    private const string JavaHomeSelectionProperty = "javaHomeSelection";
    private const string JdkUserInputPathProperty = "jdkUserInputPath";
    private const string MavenVersionSelectionProperty = "mavenVersionSelection";
    private const string MavenPathProperty = "mavenPath";
    private const string MavenSetM2HomeProperty = "mavenSetM2Home";
    private const string MavenOptsProperty = "mavenOpts";
    private const string MavenFeedAuthenticateProperty = "mavenFeedAuthenticate";
    private const string SqAnalysisEnabledProperty = "sqAnalysisEnabled";
    private const string CheckstyleAnalysisEnabledProperty = "checkstyleAnalysisEnabled";
    private const string PmdAnalysisEnabledProperty = "pmdAnalysisEnabled";
    private const string FindbugsAnalysisEnabledProperty = "findbugsAnalysisEnabled";

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenTaskBase"/> class.
    /// </summary>
    /// <param name="taskVersion">The fully qualified Azure DevOps task identity, such as <c>Maven@4</c>.</param>
    protected MavenTaskBase(string taskVersion)
        : base(taskVersion)
    {
    }

    /// <summary>
    /// Gets or sets the relative path from the repository root to the Maven POM file.
    /// Azure DevOps defaults this input to <c>pom.xml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MavenPOMFile
    {
        get => GetExpression<string>(MavenPOMFileProperty);
        init => SetProperty(MavenPOMFileProperty, value);
    }

    /// <summary>
    /// Gets or sets the Maven goals to execute.
    /// Azure DevOps defaults this input to <c>package</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Goals
    {
        get => GetExpression<string>(GoalsProperty);
        init => SetProperty(GoalsProperty, value);
    }

    /// <summary>
    /// Gets or sets additional Maven command-line options.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Options
    {
        get => GetExpression<string>(OptionsProperty);
        init => SetProperty(OptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether JUnit test results produced by the Maven build should be published.
    /// Azure DevOps defaults this input to <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishJUnitResults
    {
        get => GetExpression<bool>(PublishJUnitResultsProperty);
        init => SetProperty(PublishJUnitResultsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimatch pattern for test result files.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="PublishJUnitResults"/> is <c>true</c>.
    /// The default pattern differs by task major version.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? TestResultsFiles
    {
        get => GetExpression<string>(TestResultsFilesProperty);
        init => SetProperty(TestResultsFilesProperty, value);
    }

    /// <summary>
    /// Gets or sets the test run title used when publishing JUnit test results.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="PublishJUnitResults"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>(TestRunTitleProperty);
        init => SetProperty(TestRunTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the Maven task's built-in code coverage integration.
    /// Azure DevOps defaults this input to <see cref="MavenCodeCoverageTool.None"/>.
    /// </summary>
    [YamlIgnore]
    public MavenCodeCoverageTool CodeCoverageTool
    {
        get => GetEnum(CodeCoverageToolProperty, MavenCodeCoverageTool.None);
        init => SetProperty(CodeCoverageToolProperty, value);
    }

    /// <summary>
    /// Gets or sets the comma-separated class inclusion and exclusion filters for code coverage collection.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="CodeCoverageTool"/> is not <see cref="MavenCodeCoverageTool.None"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? ClassFilter
    {
        get => GetExpression<string>(ClassFilterProperty);
        init => SetProperty(ClassFilterProperty, value);
    }

    /// <summary>
    /// Gets or sets the comma-separated directories that contain compiled class files or archives for JaCoCo coverage reporting.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="CodeCoverageTool"/> is <see cref="MavenCodeCoverageTool.JaCoCo"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? ClassFilesDirectories
    {
        get => GetExpression<string>(ClassFilesDirectoriesProperty);
        init => SetProperty(ClassFilesDirectoriesProperty, value);
    }

    /// <summary>
    /// Gets or sets the comma-separated source directories used to map JaCoCo coverage back to source files.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="CodeCoverageTool"/> is <see cref="MavenCodeCoverageTool.JaCoCo"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SrcDirectories
    {
        get => GetExpression<string>(SrcDirectoriesProperty);
        init => SetProperty(SrcDirectoriesProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should fail when no coverage results are produced.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="CodeCoverageTool"/> is not <see cref="MavenCodeCoverageTool.None"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? FailIfCoverageEmpty
    {
        get => GetExpression<bool>(FailIfCoverageEmptyProperty);
        init => SetProperty(FailIfCoverageEmptyProperty, value);
    }

    /// <summary>
    /// Gets or sets how the task should determine <c>JAVA_HOME</c>.
    /// Azure DevOps defaults this input to <see cref="MavenJavaHomeSelection.JDKVersion"/>.
    /// </summary>
    [YamlIgnore]
    public MavenJavaHomeSelection JavaHomeSelection
    {
        get => GetEnum(JavaHomeSelectionProperty, MavenJavaHomeSelection.JDKVersion);
        init => SetProperty(JavaHomeSelectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the custom JDK path to assign to <c>JAVA_HOME</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="JavaHomeSelection"/> is <see cref="MavenJavaHomeSelection.Path"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? JdkUserInputPath
    {
        get => GetExpression<string>(JdkUserInputPathProperty);
        init => SetProperty(JdkUserInputPathProperty, value);
    }

    /// <summary>
    /// Gets or sets how the task should choose the Maven installation.
    /// Azure DevOps defaults this input to <see cref="MavenVersionSelection.Default"/>.
    /// </summary>
    [YamlIgnore]
    public MavenVersionSelection MavenVersionSelection
    {
        get => GetEnum(MavenVersionSelectionProperty, Tasks.MavenVersionSelection.Default);
        init => SetProperty(MavenVersionSelectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the custom Maven installation path.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="MavenVersionSelection"/> is <see cref="Tasks.MavenVersionSelection.Path"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? MavenPath
    {
        get => GetExpression<string>(MavenPathProperty);
        init => SetProperty(MavenPathProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should set the <c>M2_HOME</c> variable for a custom Maven installation.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="MavenVersionSelection"/> is <see cref="Tasks.MavenVersionSelection.Path"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? MavenSetM2Home
    {
        get => GetExpression<bool>(MavenSetM2HomeProperty);
        init => SetProperty(MavenSetM2HomeProperty, value);
    }

    /// <summary>
    /// Gets or sets the <c>MAVEN_OPTS</c> environment variable value used by the task.
    /// Azure DevOps defaults this input to <c>-Xmx1024m</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MavenOpts
    {
        get => GetExpression<string>(MavenOptsProperty);
        init => SetProperty(MavenOptsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should authenticate automatically with Azure Artifacts Maven feeds.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? MavenFeedAuthenticate
    {
        get => GetExpression<bool>(MavenFeedAuthenticateProperty);
        init => SetProperty(MavenFeedAuthenticateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether SonarQube or SonarCloud analysis should run.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SqAnalysisEnabled
    {
        get => GetExpression<bool>(SqAnalysisEnabledProperty);
        init => SetProperty(SqAnalysisEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should run Checkstyle and publish its results as artifacts.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckstyleAnalysisEnabled
    {
        get => GetExpression<bool>(CheckstyleAnalysisEnabledProperty);
        init => SetProperty(CheckstyleAnalysisEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should run PMD and publish its results as artifacts.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PmdAnalysisEnabled
    {
        get => GetExpression<bool>(PmdAnalysisEnabledProperty);
        init => SetProperty(PmdAnalysisEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the task should run FindBugs and publish its results as artifacts.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FindbugsAnalysisEnabled
    {
        get => GetExpression<bool>(FindbugsAnalysisEnabledProperty);
        init => SetProperty(FindbugsAnalysisEnabledProperty, value);
    }
}

/// <summary>
/// Represents the deprecated <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/maven-v1?view=azure-pipelines">Maven@1</see>
/// Azure DevOps task. The version-matched reference used for the v1-only properties is the Microsoft Learn task reference,
/// and the current Azure Pipelines task family specification is maintained in
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/MavenV2/task.json">MavenV2/task.json</see>
/// for later majors in the same family.
/// </summary>
/// <remarks>
/// Azure DevOps marks Maven@1 as deprecated. Use <see cref="MavenTask"/> for new pipelines.
/// </remarks>
public record MavenV1Task : MavenTaskBase
{
    private const string JdkVersionProperty = "jdkVersion";
    private const string JdkArchitectureProperty = "jdkArchitecture";
    private const string SonarQubeServiceEndpointProperty = "sonarQubeServiceEndpoint";
    private const string SonarQubeProjectNameProperty = "sonarQubeProjectName";
    private const string SonarQubeProjectKeyProperty = "sonarQubeProjectKey";
    private const string SonarQubeProjectVersionProperty = "sonarQubeProjectVersion";
    private const string SonarQubeSpecifyDBProperty = "sonarQubeSpecifyDB";
    private const string SonarQubeDBUrlProperty = "sonarQubeDBUrl";
    private const string SonarQubeDBUsernameProperty = "sonarQubeDBUsername";
    private const string SonarQubeDBPasswordProperty = "sonarQubeDBPassword";
    private const string SonarQubeIncludeFullReportProperty = "sonarQubeIncludeFullReport";
    private const string SonarQubeFailWhenQualityGateFailsProperty = "sonarQubeFailWhenQualityGateFails";

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenV1Task"/> class.
    /// </summary>
    public MavenV1Task()
        : base("Maven@1")
    {
    }

    /// <summary>
    /// Gets or sets the JDK version discovered by the task.
    /// Azure DevOps defaults this input to <see cref="MavenV1JdkVersion.Default"/>.
    /// </summary>
    [YamlIgnore]
    public MavenV1JdkVersion JdkVersion
    {
        get => GetEnum(JdkVersionProperty, MavenV1JdkVersion.Default);
        init => SetProperty(JdkVersionProperty, value);
    }

    /// <summary>
    /// Gets or sets the JDK architecture discovered by the task.
    /// Azure DevOps defaults this input to <see cref="MavenV1JdkArchitecture.X64"/>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="JdkVersion"/> is not <see cref="MavenV1JdkVersion.Default"/>.
    /// </remarks>
    [YamlIgnore]
    public MavenV1JdkArchitecture JdkArchitecture
    {
        get => GetEnum(JdkArchitectureProperty, MavenV1JdkArchitecture.X64);
        init => SetProperty(JdkArchitectureProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube generic service endpoint.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeServiceEndpoint
    {
        get => GetExpression<string>(SonarQubeServiceEndpointProperty);
        init => SetProperty(SonarQubeServiceEndpointProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube project name.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeProjectName
    {
        get => GetExpression<string>(SonarQubeProjectNameProperty);
        init => SetProperty(SonarQubeProjectNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube project key.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeProjectKey
    {
        get => GetExpression<string>(SonarQubeProjectKeyProperty);
        init => SetProperty(SonarQubeProjectKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube project version.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeProjectVersion
    {
        get => GetExpression<string>(SonarQubeProjectVersionProperty);
        init => SetProperty(SonarQubeProjectVersionProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether legacy SonarQube database connection details should be supplied.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? SonarQubeSpecifyDB
    {
        get => GetExpression<bool>(SonarQubeSpecifyDBProperty);
        init => SetProperty(SonarQubeSpecifyDBProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube database connection string.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SonarQubeSpecifyDB"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeDBUrl
    {
        get => GetExpression<string>(SonarQubeDBUrlProperty);
        init => SetProperty(SonarQubeDBUrlProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube database user name.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SonarQubeSpecifyDB"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeDBUsername
    {
        get => GetExpression<string>(SonarQubeDBUsernameProperty);
        init => SetProperty(SonarQubeDBUsernameProperty, value);
    }

    /// <summary>
    /// Gets or sets the SonarQube database password.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SonarQubeSpecifyDB"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SonarQubeDBPassword
    {
        get => GetExpression<string>(SonarQubeDBPasswordProperty);
        init => SetProperty(SonarQubeDBPasswordProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the full SonarQube analysis report should be included in the build summary.
    /// Azure DevOps defaults this input to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? SonarQubeIncludeFullReport
    {
        get => GetExpression<bool>(SonarQubeIncludeFullReportProperty);
        init => SetProperty(SonarQubeIncludeFullReportProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the build should fail when the SonarQube quality gate fails.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? SonarQubeFailWhenQualityGateFails
    {
        get => GetExpression<bool>(SonarQubeFailWhenQualityGateFailsProperty);
        init => SetProperty(SonarQubeFailWhenQualityGateFailsProperty, value);
    }
}

/// <summary>
/// Represents the deprecated <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/maven-v2?view=azure-pipelines">Maven@2</see>
/// Azure DevOps task. The official task implementation is defined in
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/MavenV2/task.json">Tasks/MavenV2/task.json</see>.
/// </summary>
/// <remarks>
/// Azure DevOps marks Maven@2 as deprecated. Use <see cref="MavenTask"/> for new pipelines.
/// </remarks>
public record MavenV2Task : MavenTaskBase
{
    private const string AllowBrokenSymbolicLinksProperty = "allowBrokenSymbolicLinks";
    private const string RestoreOriginalPomXmlProperty = "restoreOriginalPomXml";
    private const string JdkVersionProperty = "jdkVersion";
    private const string JdkArchitectureProperty = "jdkArchitecture";
    private const string IsJacocoCoverageReportXMLProperty = "isJacocoCoverageReportXML";
    private const string SqMavenPluginVersionChoiceProperty = "sqMavenPluginVersionChoice";

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenV2Task"/> class.
    /// </summary>
    public MavenV2Task()
        : this("Maven@2")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenV2Task"/> class with a specific task major version.
    /// </summary>
    /// <param name="taskVersion">The fully qualified Azure DevOps task identity.</param>
    protected MavenV2Task(string taskVersion)
        : base(taskVersion)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether broken symbolic links should be tolerated while publishing test results.
    /// Azure DevOps defaults this input to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="PublishJUnitResults"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? AllowBrokenSymbolicLinks
    {
        get => GetExpression<bool>(AllowBrokenSymbolicLinksProperty);
        init => SetProperty(AllowBrokenSymbolicLinksProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the original <c>pom.xml</c> should be restored after the task modifies it for code coverage.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="CodeCoverageTool"/> is not <see cref="MavenCodeCoverageTool.None"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? RestoreOriginalPomXml
    {
        get => GetExpression<bool>(RestoreOriginalPomXmlProperty);
        init => SetProperty(RestoreOriginalPomXmlProperty, value);
    }

    /// <summary>
    /// Gets or sets the JDK version discovered by the task.
    /// Azure DevOps defaults this input to <see cref="MavenJdkVersion.Default"/>.
    /// </summary>
    [YamlIgnore]
    public MavenJdkVersion JdkVersion
    {
        get => GetEnum(JdkVersionProperty, MavenJdkVersion.Default);
        init => SetProperty(JdkVersionProperty, value);
    }

    /// <summary>
    /// Gets or sets the JDK architecture discovered by the task.
    /// Azure DevOps defaults this input to <see cref="MavenJdkArchitecture.X64"/>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="JdkVersion"/> is not <see cref="MavenJdkVersion.Default"/>.
    /// </remarks>
    [YamlIgnore]
    public MavenJdkArchitecture JdkArchitecture
    {
        get => GetEnum(JdkArchitectureProperty, MavenJdkArchitecture.X64);
        init => SetProperty(JdkArchitectureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether SonarQube analysis should consume JaCoCo XML reports.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c> and <see cref="CodeCoverageTool"/> is <see cref="MavenCodeCoverageTool.JaCoCo"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? IsJacocoCoverageReportXML
    {
        get => GetExpression<bool>(IsJacocoCoverageReportXMLProperty);
        init => SetProperty(IsJacocoCoverageReportXMLProperty, value);
    }

    /// <summary>
    /// Gets or sets which SonarQube Maven plugin version should be used.
    /// Azure DevOps defaults this input to <see cref="MavenSonarQubeMavenPluginVersionChoice.Latest"/>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SqAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public MavenSonarQubeMavenPluginVersionChoice SqMavenPluginVersionChoice
    {
        get => GetEnum(SqMavenPluginVersionChoiceProperty, MavenSonarQubeMavenPluginVersionChoice.Latest);
        init => SetProperty(SqMavenPluginVersionChoiceProperty, value);
    }
}

/// <summary>
/// Represents the deprecated <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/maven-v3?view=azure-pipelines">Maven@3</see>
/// Azure DevOps task. The official task implementation is defined in
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/MavenV3/task.json">Tasks/MavenV3/task.json</see>.
/// </summary>
/// <remarks>
/// Azure DevOps marks Maven@3 as deprecated. Use <see cref="MavenTask"/> for new pipelines.
/// </remarks>
public record MavenV3Task : MavenV2Task
{
    private const string SkipEffectivePomProperty = "skipEffectivePom";
    private const string SpotBugsAnalysisEnabledProperty = "spotBugsAnalysisEnabled";
    private const string SpotBugsMavenPluginVersionProperty = "spotBugsMavenPluginVersion";
    private const string SpotBugsGoalProperty = "spotBugsGoal";
    private const string SpotBugsFailWhenBugsFoundProperty = "spotBugsFailWhenBugsFound";

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenV3Task"/> class.
    /// </summary>
    public MavenV3Task()
        : this("Maven@3")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenV3Task"/> class with a specific task major version.
    /// </summary>
    /// <param name="taskVersion">The fully qualified Azure DevOps task identity.</param>
    protected MavenV3Task(string taskVersion)
        : base(taskVersion)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether generating the effective POM should be skipped while authenticating with Azure Artifacts feeds.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SkipEffectivePom
    {
        get => GetExpression<bool>(SkipEffectivePomProperty);
        init => SetProperty(SkipEffectivePomProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the SpotBugs Maven plugin should run.
    /// Azure DevOps defaults this input to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SpotBugsAnalysisEnabled
    {
        get => GetExpression<bool>(SpotBugsAnalysisEnabledProperty);
        init => SetProperty(SpotBugsAnalysisEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the SpotBugs Maven plugin version.
    /// Azure DevOps defaults this input to <c>4.5.3.0</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SpotBugsAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? SpotBugsMavenPluginVersion
    {
        get => GetExpression<string>(SpotBugsMavenPluginVersionProperty);
        init => SetProperty(SpotBugsMavenPluginVersionProperty, value);
    }

    /// <summary>
    /// Gets or sets the SpotBugs Maven goal to run.
    /// Azure DevOps defaults this input to <see cref="MavenSpotBugsGoal.SpotBugs"/>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SpotBugsAnalysisEnabled"/> is <c>true</c>.
    /// </remarks>
    [YamlIgnore]
    public MavenSpotBugsGoal SpotBugsGoal
    {
        get => GetEnum(SpotBugsGoalProperty, MavenSpotBugsGoal.SpotBugs);
        init => SetProperty(SpotBugsGoalProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the build should fail when <c>spotbugs:check</c> finds bugs.
    /// Azure DevOps defaults this input to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This input only applies when <see cref="SpotBugsAnalysisEnabled"/> is <c>true</c> and <see cref="SpotBugsGoal"/> is <see cref="Tasks.MavenSpotBugsGoal.Check"/>.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<bool>? SpotBugsFailWhenBugsFound
    {
        get => GetExpression<bool>(SpotBugsFailWhenBugsFoundProperty);
        init => SetProperty(SpotBugsFailWhenBugsFoundProperty, value);
    }
}

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/maven-v4?view=azure-pipelines">Maven@4</see>
/// Azure DevOps task. The official task implementation is defined in
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/MavenV4/task.json">Tasks/MavenV4/task.json</see>.
/// </summary>
public record MavenTask : MavenV3Task
{
    private const string ConnectedServiceNameProperty = "ConnectedServiceName";

    /// <summary>
    /// Initializes a new instance of the <see cref="MavenTask"/> class.
    /// </summary>
    public MavenTask()
        : base("Maven@4")
    {
    }

    /// <summary>
    /// Gets or sets the Azure Resource Manager service connection.
    /// The official YAML alias is <c>azureSubscription</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectedServiceName
    {
        get => GetExpression<string>(ConnectedServiceNameProperty);
        init => SetProperty(ConnectedServiceNameProperty, value);
    }
}

/// <summary>
/// Built-in code coverage integrations supported by the Maven task family.
/// </summary>
public enum MavenCodeCoverageTool
{
    /// <summary>
    /// Disable the task's built-in code coverage handling.
    /// </summary>
    [YamlMember(Alias = "None")]
    None,

    /// <summary>
    /// Use Cobertura coverage reporting.
    /// </summary>
    [YamlMember(Alias = "Cobertura")]
    Cobertura,

    /// <summary>
    /// Use JaCoCo coverage reporting.
    /// </summary>
    [YamlMember(Alias = "JaCoCo")]
    JaCoCo,
}

/// <summary>
/// Options for configuring <c>JAVA_HOME</c> in the Maven task family.
/// </summary>
public enum MavenJavaHomeSelection
{
    /// <summary>
    /// Discover a JDK installation by version.
    /// </summary>
    [YamlMember(Alias = "JDKVersion")]
    JDKVersion,

    /// <summary>
    /// Use an explicit JDK path.
    /// </summary>
    [YamlMember(Alias = "Path")]
    Path,
}

/// <summary>
/// JDK version options supported by Maven@2, Maven@3, and Maven@4.
/// </summary>
public enum MavenJdkVersion
{
    /// <summary>
    /// Use the task's default JDK discovery behavior.
    /// </summary>
    [YamlMember(Alias = "default")]
    Default,

    /// <summary>
    /// JDK 21.
    /// </summary>
    [YamlMember(Alias = "1.21")]
    Jdk21,

    /// <summary>
    /// JDK 17.
    /// </summary>
    [YamlMember(Alias = "1.17")]
    Jdk17,

    /// <summary>
    /// JDK 11.
    /// </summary>
    [YamlMember(Alias = "1.11")]
    Jdk11,

    /// <summary>
    /// JDK 10.
    /// </summary>
    [YamlMember(Alias = "1.10")]
    Jdk10,

    /// <summary>
    /// JDK 9.
    /// </summary>
    [YamlMember(Alias = "1.9")]
    Jdk9,

    /// <summary>
    /// JDK 8.
    /// </summary>
    [YamlMember(Alias = "1.8")]
    Jdk8,

    /// <summary>
    /// JDK 7.
    /// </summary>
    [YamlMember(Alias = "1.7")]
    Jdk7,

    /// <summary>
    /// JDK 6.
    /// </summary>
    [YamlMember(Alias = "1.6")]
    Jdk6,
}

/// <summary>
/// JDK version options supported by Maven@1.
/// </summary>
public enum MavenV1JdkVersion
{
    /// <summary>
    /// Use the task's default JDK discovery behavior.
    /// </summary>
    [YamlMember(Alias = "default")]
    Default,

    /// <summary>
    /// JDK 9.
    /// </summary>
    [YamlMember(Alias = "1.9")]
    Jdk9,

    /// <summary>
    /// JDK 8.
    /// </summary>
    [YamlMember(Alias = "1.8")]
    Jdk8,

    /// <summary>
    /// JDK 7.
    /// </summary>
    [YamlMember(Alias = "1.7")]
    Jdk7,

    /// <summary>
    /// JDK 6.
    /// </summary>
    [YamlMember(Alias = "1.6")]
    Jdk6,
}

/// <summary>
/// JDK architecture options supported by Maven@2, Maven@3, and Maven@4.
/// </summary>
public enum MavenJdkArchitecture
{
    /// <summary>
    /// x86 architecture.
    /// </summary>
    [YamlMember(Alias = "x86")]
    X86,

    /// <summary>
    /// x64 architecture.
    /// </summary>
    [YamlMember(Alias = "x64")]
    X64,

    /// <summary>
    /// arm64 architecture.
    /// </summary>
    [YamlMember(Alias = "arm64")]
    Arm64,
}

/// <summary>
/// JDK architecture options supported by Maven@1.
/// </summary>
public enum MavenV1JdkArchitecture
{
    /// <summary>
    /// x86 architecture.
    /// </summary>
    [YamlMember(Alias = "x86")]
    X86,

    /// <summary>
    /// x64 architecture.
    /// </summary>
    [YamlMember(Alias = "x64")]
    X64,
}

/// <summary>
/// Options for selecting the Maven installation used by the task.
/// </summary>
public enum MavenVersionSelection
{
    /// <summary>
    /// Use the agent's default Maven installation.
    /// </summary>
    [YamlMember(Alias = "Default")]
    Default,

    /// <summary>
    /// Use a Maven installation from a custom path.
    /// </summary>
    [YamlMember(Alias = "Path")]
    Path,
}

/// <summary>
/// SonarQube Maven plugin version choices supported by Maven@2 and later.
/// </summary>
public enum MavenSonarQubeMavenPluginVersionChoice
{
    /// <summary>
    /// Use the latest available SonarQube Maven plugin.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Use the SonarQube Maven plugin version declared in <c>pom.xml</c>.
    /// </summary>
    [YamlMember(Alias = "pom")]
    Pom,
}

/// <summary>
/// SpotBugs Maven goals supported by Maven@3 and later.
/// </summary>
public enum MavenSpotBugsGoal
{
    /// <summary>
    /// Run <c>spotbugs</c> to generate a report.
    /// </summary>
    [YamlMember(Alias = "spotbugs")]
    SpotBugs,

    /// <summary>
    /// Run <c>check</c> to fail the build when bugs are found.
    /// </summary>
    [YamlMember(Alias = "check")]
    Check,
}
