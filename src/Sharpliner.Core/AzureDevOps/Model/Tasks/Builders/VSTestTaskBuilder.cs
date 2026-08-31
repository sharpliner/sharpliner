using System;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating strongly typed Visual Studio Test tasks.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/vstest-v3?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public class VSTestTaskBuilder
{
    /// <summary>
    /// Gets a builder targeting <c>VSTest@3</c>.
    /// </summary>
    public VSTestSelectionBuilder<VSTestTask> V3 => new(() => new VSTestTask());

    /// <summary>
    /// Gets a builder targeting <c>VSTest@2</c>.
    /// </summary>
    public VSTestSelectionBuilder<VSTestV2Task> V2 => new(() => new VSTestV2Task());

    /// <summary>
    /// Creates a <c>VSTest@1</c> task.
    /// </summary>
    /// <param name="testAssembly">Test assembly pattern to execute.</param>
    public VSTestV1Task V1(AdoExpression<string> testAssembly) => new(testAssembly);

    /// <summary>
    /// Creates a <c>VSTest@3</c> task selecting tests from assemblies.
    /// </summary>
    /// <param name="testFiles">Multiline minimatch test file patterns.</param>
    public VSTestExecutionBuilder<VSTestTask> TestAssemblies(AdoExpression<string> testFiles) => V3.TestAssemblies(testFiles);

    /// <summary>
    /// Creates a <c>VSTest@3</c> task selecting tests from a test plan.
    /// </summary>
    /// <param name="testPlan">Test plan ID.</param>
    /// <param name="testSuite">Test suite ID(s).</param>
    /// <param name="testConfiguration">Test configuration ID.</param>
    public VSTestExecutionBuilder<VSTestTask> TestPlan(AdoExpression<string> testPlan, AdoExpression<string> testSuite, AdoExpression<string> testConfiguration)
        => V3.TestPlan(testPlan, testSuite, testConfiguration);

    /// <summary>
    /// Creates a <c>VSTest@3</c> task selecting tests from an existing test run.
    /// </summary>
    /// <param name="testRunId">Test run ID.</param>
    public VSTestExecutionBuilder<VSTestTask> TestRun(AdoExpression<string>? testRunId = null)
        => V3.TestRun(testRunId);
}

/// <summary>
/// Fluent builder for valid VSTest selection and test platform subsets.
/// </summary>
/// <typeparam name="TTask">Task major model type.</typeparam>
public class VSTestSelectionBuilder<TTask>
    where TTask : VSTestV2AndV3Task
{
    private readonly Func<TTask> _taskFactory;

    internal VSTestSelectionBuilder(Func<TTask> taskFactory)
    {
        _taskFactory = taskFactory;
    }

    /// <summary>
    /// Selects tests from assemblies.
    /// </summary>
    /// <param name="testFiles">Multiline minimatch test file patterns.</param>
    public VSTestExecutionBuilder<TTask> TestAssemblies(AdoExpression<string> testFiles)
        => new(_taskFactory() with
        {
            TestSelector = VSTestSelection.TestAssemblies,
            TestAssemblyVer2 = testFiles,
        });

    /// <summary>
    /// Selects tests from a test plan.
    /// </summary>
    /// <param name="testPlan">Test plan ID.</param>
    /// <param name="testSuite">Test suite ID(s).</param>
    /// <param name="testConfiguration">Test configuration ID.</param>
    public VSTestExecutionBuilder<TTask> TestPlan(AdoExpression<string> testPlan, AdoExpression<string> testSuite, AdoExpression<string> testConfiguration)
        => new(_taskFactory() with
        {
            TestSelector = VSTestSelection.TestPlan,
            TestPlan = testPlan,
            TestSuite = testSuite,
            TestConfiguration = testConfiguration,
        });

    /// <summary>
    /// Selects tests from an existing test run.
    /// </summary>
    /// <param name="testRunId">Test run ID. Defaults to <c>$(test.RunId)</c>.</param>
    public VSTestExecutionBuilder<TTask> TestRun(AdoExpression<string>? testRunId = null)
        => new(_taskFactory() with
        {
            TestSelector = VSTestSelection.TestRun,
            TcmTestRun = testRunId ?? "$(test.RunId)",
        });
}

/// <summary>
/// Fluent builder for valid VSTest execution platform subsets.
/// </summary>
/// <typeparam name="TTask">Task major model type.</typeparam>
public class VSTestExecutionBuilder<TTask>
    where TTask : VSTestV2AndV3Task
{
    private readonly TTask _task;

    internal VSTestExecutionBuilder(TTask task)
    {
        _task = task;
    }

    /// <summary>
    /// Uses a built-in platform version.
    /// </summary>
    /// <param name="version">Test platform version. Defaults to <see cref="VSTestVersion.Latest"/>.</param>
    public TTask UsingPlatformVersion(VSTestVersion version = VSTestVersion.Latest)
        => _task with
        {
            VstestLocationMethod = VSTestLocationMethod.Version,
            VsTestVersion = version,
            VstestLocation = null,
        };

    /// <summary>
    /// Uses an explicit path to <c>vstest.console.exe</c>.
    /// </summary>
    /// <param name="vstestLocation">Path to the test platform executable.</param>
    public TTask UsingPlatformLocation(AdoExpression<string> vstestLocation)
        => _task with
        {
            VstestLocationMethod = VSTestLocationMethod.Location,
            VsTestVersion = null,
            VstestLocation = vstestLocation,
        };

    /// <summary>
    /// Builds the task without setting explicit platform selection inputs.
    /// </summary>
    public TTask Build() => _task;
}
