using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base model for the Visual Studio Test task majors 2 and 3.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v3?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record VSTestV2AndV3Task : AzureDevOpsTask
{
    /// <summary>
    /// Selects how tests are discovered.
    /// Allowed values: <see cref="VSTestSelection"/>.
    /// Required.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestSelection>? TestSelector
    {
        get => GetExpression<VSTestSelection>("testSelector");
        init => SetProperty("testSelector", value);
    }

    /// <summary>
    /// Test files (supports multiline minimatch patterns).
    /// Required when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestAssemblies"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestAssemblyVer2
    {
        get => GetExpression<string>("testAssemblyVer2");
        init => SetProperty("testAssemblyVer2", value);
    }

    /// <summary>
    /// Test plan ID.
    /// Required when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestPlan"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestPlan
    {
        get => GetExpression<string>("testPlan");
        init => SetProperty("testPlan", value);
    }

    /// <summary>
    /// Test suite ID(s).
    /// Required when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestPlan"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestSuite
    {
        get => GetExpression<string>("testSuite");
        init => SetProperty("testSuite", value);
    }

    /// <summary>
    /// Test configuration ID.
    /// Required when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestPlan"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestConfiguration
    {
        get => GetExpression<string>("testConfiguration");
        init => SetProperty("testConfiguration", value);
    }

    /// <summary>
    /// Test run ID to execute.
    /// Used when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestRun"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TcmTestRun
    {
        get => GetExpression<string>("tcmTestRun");
        init => SetProperty("tcmTestRun", value);
    }

    /// <summary>
    /// Search folder for test files.
    /// Default value: <c>$(System.DefaultWorkingDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SearchFolder
    {
        get => GetExpression<string>("searchFolder");
        init => SetProperty("searchFolder", value);
    }

    /// <summary>
    /// Folder where test results are stored.
    /// Default value: <c>$(Agent.TempDirectory)\TestResults</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResultsFolder
    {
        get => GetExpression<string>("resultsFolder");
        init => SetProperty("resultsFolder", value);
    }

    /// <summary>
    /// Additional criteria to filter tests.
    /// Used when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestAssemblies"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestFilterCriteria
    {
        get => GetExpression<string>("testFiltercriteria");
        init => SetProperty("testFiltercriteria", value);
    }

    /// <summary>
    /// Runs only impacted tests (Test Impact Analysis).
    /// Used when <see cref="TestSelector"/> is <see cref="VSTestSelection.TestAssemblies"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RunOnlyImpactedTests
    {
        get => GetExpression<bool>("runOnlyImpactedTests");
        init => SetProperty("runOnlyImpactedTests", value);
    }

    /// <summary>
    /// Number of builds after which all tests are run.
    /// Used when <see cref="RunOnlyImpactedTests"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RunAllTestsAfterXBuilds
    {
        get => GetExpression<string>("runAllTestsAfterXBuilds");
        init => SetProperty("runAllTestsAfterXBuilds", value);
    }

    /// <summary>
    /// Indicates test mix contains UI tests.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UiTests
    {
        get => GetExpression<bool>("uiTests");
        init => SetProperty("uiTests", value);
    }

    /// <summary>
    /// Selects test platform by version or explicit location.
    /// Default value: <see cref="VSTestLocationMethod.Version"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestLocationMethod>? VstestLocationMethod
    {
        get => GetExpression<VSTestLocationMethod>("vstestLocationMethod");
        init => SetProperty("vstestLocationMethod", value);
    }

    /// <summary>
    /// Test platform version.
    /// Used when <see cref="VstestLocationMethod"/> is <see cref="VSTestLocationMethod.Version"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestVersion>? VsTestVersion
    {
        get => GetExpression<VSTestVersion>("vsTestVersion");
        init => SetProperty("vsTestVersion", value);
    }

    /// <summary>
    /// Path to <c>vstest.console.exe</c>.
    /// Used when <see cref="VstestLocationMethod"/> is <see cref="VSTestLocationMethod.Location"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstestLocation
    {
        get => GetExpression<string>("vstestLocation");
        init => SetProperty("vstestLocation", value);
    }

    /// <summary>
    /// Path to a runsettings or testsettings file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RunSettingsFile
    {
        get => GetExpression<string>("runSettingsFile");
        init => SetProperty("runSettingsFile", value);
    }

    /// <summary>
    /// Overrides values in <c>TestRunParameters</c> (runsettings) or <c>Properties</c> (testsettings).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideTestrunParameters
    {
        get => GetExpression<string>("overrideTestrunParameters");
        init => SetProperty("overrideTestrunParameters", value);
    }

    /// <summary>
    /// Directory path to custom test adapters.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PathToCustomTestAdapters
    {
        get => GetExpression<string>("pathtoCustomTestAdapters");
        init => SetProperty("pathtoCustomTestAdapters", value);
    }

    /// <summary>
    /// Runs tests in parallel on multi-core machines.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RunInParallel
    {
        get => GetExpression<bool>("runInParallel");
        init => SetProperty("runInParallel", value);
    }

    /// <summary>
    /// Runs tests in an isolated process.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RunTestsInIsolation
    {
        get => GetExpression<bool>("runTestsInIsolation");
        init => SetProperty("runTestsInIsolation", value);
    }

    /// <summary>
    /// Enables code coverage collection.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CodeCoverageEnabled
    {
        get => GetExpression<bool>("codeCoverageEnabled");
        init => SetProperty("codeCoverageEnabled", value);
    }

    /// <summary>
    /// Additional command-line options for <c>vstest.console.exe</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OtherConsoleOptions
    {
        get => GetExpression<string>("otherConsoleOptions");
        init => SetProperty("otherConsoleOptions", value);
    }

    /// <summary>
    /// Test distribution strategy across agents.
    /// Default value: <see cref="VSTestDistributionBatchType.BasedOnTestCases"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestDistributionBatchType>? DistributionBatchType
    {
        get => GetExpression<VSTestDistributionBatchType>("distributionBatchType");
        init => SetProperty("distributionBatchType", value);
    }

    /// <summary>
    /// Batch options when distribution is based on test count.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestBatchingBasedOnAgentsOption>? BatchingBasedOnAgentsOption
    {
        get => GetExpression<VSTestBatchingBasedOnAgentsOption>("batchingBasedOnAgentsOption");
        init => SetProperty("batchingBasedOnAgentsOption", value);
    }

    /// <summary>
    /// Number of tests per batch.
    /// Used when <see cref="DistributionBatchType"/> is <see cref="VSTestDistributionBatchType.BasedOnTestCases"/> and custom batch size is selected.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomBatchSizeValue
    {
        get => GetExpression<string>("customBatchSizeValue");
        init => SetProperty("customBatchSizeValue", value);
    }

    /// <summary>
    /// Batch options when distribution is based on historical execution time.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestBatchingBasedOnExecutionTimeOption>? BatchingBasedOnExecutionTimeOption
    {
        get => GetExpression<VSTestBatchingBasedOnExecutionTimeOption>("batchingBasedOnExecutionTimeOption");
        init => SetProperty("batchingBasedOnExecutionTimeOption", value);
    }

    /// <summary>
    /// Target runtime in seconds per batch.
    /// Used when <see cref="DistributionBatchType"/> is <see cref="VSTestDistributionBatchType.BasedOnExecutionTime"/> and custom time batch size is selected.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomRunTimePerBatchValue
    {
        get => GetExpression<string>("customRunTimePerBatchValue");
        init => SetProperty("customRunTimePerBatchValue", value);
    }

    /// <summary>
    /// Replicates tests on each agent instead of distributing across agents.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DontDistribute
    {
        get => GetExpression<bool>("dontDistribute");
        init => SetProperty("dontDistribute", value);
    }

    /// <summary>
    /// Name for the test run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Build platform associated with test results.
    /// Emits <c>platform</c> input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Platform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Alias for <see cref="Platform"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildPlatform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Build configuration associated with test results.
    /// Emits <c>configuration</c> input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Configuration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Alias for <see cref="Configuration"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildConfiguration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Custom logger configuration.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomLoggerConfig
    {
        get => GetExpression<string>("customLoggerConfig");
        init => SetProperty("customLoggerConfig", value);
    }

    /// <summary>
    /// Uploads test run attachments.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishRunAttachments
    {
        get => GetExpression<bool>("publishRunAttachments");
        init => SetProperty("publishRunAttachments", value);
    }

    /// <summary>
    /// Disables test result publication from this task.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DoNotPublishTestResults
    {
        get => GetExpression<bool>("donotPublishTestResults");
        init => SetProperty("donotPublishTestResults", value);
    }

    /// <summary>
    /// Fails the task when fewer than <see cref="MinimumExpectedTests"/> tests run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnMinTestsNotRun
    {
        get => GetExpression<bool>("failOnMinTestsNotRun");
        init => SetProperty("failOnMinTestsNotRun", value);
    }

    /// <summary>
    /// Minimum number of tests that should run.
    /// Used when <see cref="FailOnMinTestsNotRun"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MinimumExpectedTests
    {
        get => GetExpression<string>("minimumExpectedTests");
        init => SetProperty("minimumExpectedTests", value);
    }

    /// <summary>
    /// Collects advanced diagnostics for catastrophic failures.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DiagnosticsEnabled
    {
        get => GetExpression<bool>("diagnosticsEnabled");
        init => SetProperty("diagnosticsEnabled", value);
    }

    /// <summary>
    /// Controls process dump collection behavior.
    /// Used when <see cref="DiagnosticsEnabled"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestCollectDumpOn>? CollectDumpOn
    {
        get => GetExpression<VSTestCollectDumpOn>("collectDumpOn");
        init => SetProperty("collectDumpOn", value);
    }

    /// <summary>
    /// Reruns failed tests.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RerunFailedTests
    {
        get => GetExpression<bool>("rerunFailedTests");
        init => SetProperty("rerunFailedTests", value);
    }

    /// <summary>
    /// Threshold type for rerun suppression.
    /// Used when <see cref="RerunFailedTests"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestRerunType>? RerunType
    {
        get => GetExpression<VSTestRerunType>("rerunType");
        init => SetProperty("rerunType", value);
    }

    /// <summary>
    /// Failure percentage threshold for reruns.
    /// Used when <see cref="RerunType"/> is <see cref="VSTestRerunType.BasedOnTestFailurePercentage"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RerunFailedThreshold
    {
        get => GetExpression<string>("rerunFailedThreshold");
        init => SetProperty("rerunFailedThreshold", value);
    }

    /// <summary>
    /// Failed test count threshold for reruns.
    /// Used when <see cref="RerunType"/> is <see cref="VSTestRerunType.BasedOnTestFailureCount"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RerunFailedTestCasesMaxLimit
    {
        get => GetExpression<string>("rerunFailedTestCasesMaxLimit");
        init => SetProperty("rerunFailedTestCasesMaxLimit", value);
    }

    /// <summary>
    /// Maximum rerun attempts.
    /// Used when <see cref="RerunFailedTests"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RerunMaxAttempts
    {
        get => GetExpression<string>("rerunMaxAttempts");
        init => SetProperty("rerunMaxAttempts", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VSTestV2AndV3Task"/> class.
    /// </summary>
    /// <param name="taskVersion">Task major version identity, such as <c>VSTest@3</c>.</param>
    protected VSTestV2AndV3Task(string taskVersion)
        : base(taskVersion)
    {
        DisplayName = "Visual Studio Test";
    }
}

/// <summary>
/// Strongly typed API for <c>VSTest@3</c>.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v3?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record VSTestTask : VSTestV2AndV3Task
{
    /// <summary>
    /// Azure Resource Manager service connection.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectedServiceName
    {
        get => GetExpression<string>("ConnectedServiceName");
        init => SetProperty("ConnectedServiceName", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VSTestTask"/> class.
    /// </summary>
    public VSTestTask()
        : base("VSTest@3")
    {
    }
}

/// <summary>
/// Strongly typed API for <c>VSTest@2</c>.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record VSTestV2Task : VSTestV2AndV3Task
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VSTestV2Task"/> class.
    /// </summary>
    public VSTestV2Task()
        : base("VSTest@2")
    {
    }
}

/// <summary>
/// Strongly typed API for <c>VSTest@1</c>.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record VSTestV1Task : AzureDevOpsTask
{
    /// <summary>
    /// Test assembly pattern to execute.
    /// Required.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestAssembly
    {
        get => GetExpression<string>("testAssembly");
        init => SetProperty("testAssembly", value);
    }

    /// <summary>
    /// Additional criteria to filter tests.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestFilterCriteria
    {
        get => GetExpression<string>("testFiltercriteria");
        init => SetProperty("testFiltercriteria", value);
    }

    /// <summary>
    /// Path to runsettings file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RunSettingsFile
    {
        get => GetExpression<string>("runSettingsFile");
        init => SetProperty("runSettingsFile", value);
    }

    /// <summary>
    /// Override parameters from the settings file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideTestrunParameters
    {
        get => GetExpression<string>("overrideTestrunParameters");
        init => SetProperty("overrideTestrunParameters", value);
    }

    /// <summary>
    /// Enables code coverage collection.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CodeCoverageEnabled
    {
        get => GetExpression<bool>("codeCoverageEnabled");
        init => SetProperty("codeCoverageEnabled", value);
    }

    /// <summary>
    /// Runs tests in parallel.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RunInParallel
    {
        get => GetExpression<bool>("runInParallel");
        init => SetProperty("runInParallel", value);
    }

    /// <summary>
    /// Selects test platform by version or location.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestLocationMethod>? VstestLocationMethod
    {
        get => GetExpression<VSTestLocationMethod>("vstestLocationMethod");
        init => SetProperty("vstestLocationMethod", value);
    }

    /// <summary>
    /// Test platform version.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<VSTestV1Version>? VsTestVersion
    {
        get => GetExpression<VSTestV1Version>("vsTestVersion");
        init => SetProperty("vsTestVersion", value);
    }

    /// <summary>
    /// Path to <c>vstest.console.exe</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VstestLocation
    {
        get => GetExpression<string>("vstestLocation");
        init => SetProperty("vstestLocation", value);
    }

    /// <summary>
    /// Directory path to custom adapters.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PathToCustomTestAdapters
    {
        get => GetExpression<string>("pathtoCustomTestAdapters");
        init => SetProperty("pathtoCustomTestAdapters", value);
    }

    /// <summary>
    /// Additional command-line options for <c>vstest.console.exe</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OtherConsoleOptions
    {
        get => GetExpression<string>("otherConsoleOptions");
        init => SetProperty("otherConsoleOptions", value);
    }

    /// <summary>
    /// Name for the test run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Build platform associated with test results.
    /// Emits <c>platform</c> input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Platform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Alias for <see cref="Platform"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildPlatform
    {
        get => GetExpression<string>("platform");
        init => SetProperty("platform", value);
    }

    /// <summary>
    /// Build configuration associated with test results.
    /// Emits <c>configuration</c> input.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Configuration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Alias for <see cref="Configuration"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildConfiguration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Uploads test attachments.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishRunAttachments
    {
        get => GetExpression<bool>("publishRunAttachments");
        init => SetProperty("publishRunAttachments", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VSTestV1Task"/> class.
    /// </summary>
    /// <param name="testAssembly">Test assembly pattern to execute.</param>
    public VSTestV1Task(AdoExpression<string> testAssembly)
        : base("VSTest@1")
    {
        DisplayName = "Visual Studio Test";
        TestAssembly = testAssembly;
    }
}

/// <summary>
/// Test selection options for VSTest@2 and VSTest@3.
/// </summary>
public enum VSTestSelection
{
    /// <summary>
    /// Select tests from assemblies.
    /// </summary>
    [YamlMember(Alias = "testAssemblies")]
    TestAssemblies,

    /// <summary>
    /// Select tests from a test plan.
    /// </summary>
    [YamlMember(Alias = "testPlan")]
    TestPlan,

    /// <summary>
    /// Select tests from an existing test run.
    /// </summary>
    [YamlMember(Alias = "testRun")]
    TestRun,
}

/// <summary>
/// Selects the method used to locate the test platform.
/// </summary>
public enum VSTestLocationMethod
{
    /// <summary>
    /// Select platform by version.
    /// </summary>
    [YamlMember(Alias = "version")]
    Version,

    /// <summary>
    /// Select platform by explicit path.
    /// </summary>
    [YamlMember(Alias = "location")]
    Location,
}

/// <summary>
/// VSTest platform versions for task majors 2 and 3.
/// </summary>
public enum VSTestVersion
{
    /// <summary>
    /// Latest installed version.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Visual Studio 2026.
    /// </summary>
    [YamlMember(Alias = "18.0")]
    VisualStudio2026,

    /// <summary>
    /// Visual Studio 2022.
    /// </summary>
    [YamlMember(Alias = "17.0")]
    VisualStudio2022,

    /// <summary>
    /// Visual Studio 2019.
    /// </summary>
    [YamlMember(Alias = "16.0")]
    VisualStudio2019,

    /// <summary>
    /// Visual Studio 2017.
    /// </summary>
    [YamlMember(Alias = "15.0")]
    VisualStudio2017,

    /// <summary>
    /// Visual Studio 2015.
    /// </summary>
    [YamlMember(Alias = "14.0")]
    VisualStudio2015,

    /// <summary>
    /// Installed by Visual Studio Test Platform Installer.
    /// </summary>
    [YamlMember(Alias = "toolsInstaller")]
    ToolsInstaller,
}

/// <summary>
/// VSTest platform versions for task major 1.
/// </summary>
public enum VSTestV1Version
{
    /// <summary>
    /// Latest installed version.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Visual Studio 2015.
    /// </summary>
    [YamlMember(Alias = "14.0")]
    VisualStudio2015,

    /// <summary>
    /// Visual Studio 2013.
    /// </summary>
    [YamlMember(Alias = "12.0")]
    VisualStudio2013,
}

/// <summary>
/// Batch distribution strategies for VSTest.
/// </summary>
public enum VSTestDistributionBatchType
{
    /// <summary>
    /// Distribute based on number of test cases.
    /// </summary>
    [YamlMember(Alias = "basedOnTestCases")]
    BasedOnTestCases,

    /// <summary>
    /// Distribute based on previous execution times.
    /// </summary>
    [YamlMember(Alias = "basedOnExecutionTime")]
    BasedOnExecutionTime,

    /// <summary>
    /// Distribute by test assemblies.
    /// </summary>
    [YamlMember(Alias = "basedOnAssembly")]
    BasedOnAssembly,
}

/// <summary>
/// Batch options when distribution is based on test count.
/// </summary>
public enum VSTestBatchingBasedOnAgentsOption
{
    /// <summary>
    /// Determine batch size automatically.
    /// </summary>
    [YamlMember(Alias = "autoBatchSize")]
    AutoBatchSize,

    /// <summary>
    /// Use a custom batch size.
    /// </summary>
    [YamlMember(Alias = "customBatchSize")]
    CustomBatchSize,
}

/// <summary>
/// Batch options when distribution is based on execution time.
/// </summary>
public enum VSTestBatchingBasedOnExecutionTimeOption
{
    /// <summary>
    /// Determine time per batch automatically.
    /// </summary>
    [YamlMember(Alias = "autoBatchSize")]
    AutoBatchSize,

    /// <summary>
    /// Use a custom execution time per batch.
    /// </summary>
    [YamlMember(Alias = "customTimeBatchSize")]
    CustomTimeBatchSize,
}

/// <summary>
/// Diagnostic dump collection mode.
/// </summary>
public enum VSTestCollectDumpOn
{
    /// <summary>
    /// Collect dumps only on test abort.
    /// </summary>
    [YamlMember(Alias = "onAbortOnly")]
    OnAbortOnly,

    /// <summary>
    /// Always collect dumps.
    /// </summary>
    [YamlMember(Alias = "always")]
    Always,

    /// <summary>
    /// Never collect dumps.
    /// </summary>
    [YamlMember(Alias = "never")]
    Never,
}

/// <summary>
/// Rerun threshold mode.
/// </summary>
public enum VSTestRerunType
{
    /// <summary>
    /// Suppress reruns based on failed test percentage.
    /// </summary>
    [YamlMember(Alias = "basedOnTestFailurePercentage")]
    BasedOnTestFailurePercentage,

    /// <summary>
    /// Suppress reruns based on failed test count.
    /// </summary>
    [YamlMember(Alias = "basedOnTestFailureCount")]
    BasedOnTestFailureCount,
}
