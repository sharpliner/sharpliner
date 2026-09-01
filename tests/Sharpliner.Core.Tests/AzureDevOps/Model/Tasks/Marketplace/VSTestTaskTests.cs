using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class VSTestTaskTests
{
    [Fact]
    public Task Serialize_VSTest3_Task_Test()
    {
        var task = new VSTestTask
        {
            TestSelector = VSTestSelection.TestAssemblies,
            TestAssemblyVer2 = "**\\bin\\**\\*tests.dll",
            SearchFolder = "$(System.DefaultWorkingDirectory)",
            TestFilterCriteria = "Priority=1|Category=Unit",
            RunOnlyImpactedTests = true,
            RunAllTestsAfterXBuilds = "25",
            UiTests = false,
            VstestLocationMethod = VSTestLocationMethod.Version,
            VsTestVersion = VSTestVersion.VisualStudio2022,
            RunSettingsFile = "tests.runsettings",
            OverrideTestrunParameters = "-environment ci",
            PathToCustomTestAdapters = "adapters",
            RunInParallel = true,
            RunTestsInIsolation = true,
            CodeCoverageEnabled = true,
            OtherConsoleOptions = "/Blame",
            DistributionBatchType = VSTestDistributionBatchType.BasedOnExecutionTime,
            BatchingBasedOnExecutionTimeOption = VSTestBatchingBasedOnExecutionTimeOption.CustomTimeBatchSize,
            CustomRunTimePerBatchValue = "120",
            DontDistribute = false,
            TestRunTitle = "Unit tests",
            BuildPlatform = "x64",
            BuildConfiguration = "Release",
            CustomLoggerConfig = "trx",
            PublishRunAttachments = true,
            DoNotPublishTestResults = false,
            FailOnMinTestsNotRun = true,
            MinimumExpectedTests = "5",
            DiagnosticsEnabled = true,
            CollectDumpOn = VSTestCollectDumpOn.Always,
            RerunFailedTests = true,
            RerunType = VSTestRerunType.BasedOnTestFailureCount,
            RerunFailedTestCasesMaxLimit = "10",
            RerunMaxAttempts = "4",
            ConnectedServiceName = "AzureConnection",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_VSTest2_Task_Test()
    {
        var task = new VSTestV2Task
        {
            TestSelector = VSTestSelection.TestPlan,
            TestPlan = "42",
            TestSuite = "9,15",
            TestConfiguration = "3",
            ResultsFolder = "$(Agent.TempDirectory)\\results",
            VstestLocationMethod = VSTestLocationMethod.Location,
            VstestLocation = "C:\\tools\\vstest.console.exe",
            DistributionBatchType = VSTestDistributionBatchType.BasedOnTestCases,
            BatchingBasedOnAgentsOption = VSTestBatchingBasedOnAgentsOption.CustomBatchSize,
            CustomBatchSizeValue = "20",
            TestRunTitle = "Plan suite",
            Platform = "x86",
            Configuration = "Debug",
            PublishRunAttachments = false,
            DiagnosticsEnabled = true,
            CollectDumpOn = VSTestCollectDumpOn.OnAbortOnly,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_VSTest1_Task_Test()
    {
        var task = new VSTestV1Task("**\\bin\\**\\*test.dll")
        {
            TestFilterCriteria = "Category=Smoke",
            RunSettingsFile = "legacy.runsettings",
            OverrideTestrunParameters = "AppUrl=$(DeployUrl)",
            CodeCoverageEnabled = true,
            RunInParallel = true,
            VstestLocationMethod = VSTestLocationMethod.Version,
            VsTestVersion = VSTestV1Version.VisualStudio2015,
            PathToCustomTestAdapters = "legacy-adapters",
            OtherConsoleOptions = "/InIsolation",
            TestRunTitle = "Legacy",
            BuildPlatform = "Any CPU",
            BuildConfiguration = "Release",
            PublishRunAttachments = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
