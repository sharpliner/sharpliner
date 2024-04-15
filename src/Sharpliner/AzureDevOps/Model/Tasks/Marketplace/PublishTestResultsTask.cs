using System;
using Sharpliner.AzureDevOps.Tasks;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Model.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-test-results-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record PublishTestResultsTask : AzureDevOpsTask
{
    public PublishTestResultsTask(TestResultsFormat testResultsFormat, string testResultsFile) : base("PublishTestResults@2")
    {
        TestResultsFormat = testResultsFormat;
        TestResultsFiles = testResultsFile;
    }

    /// <summary>
    /// Specifies the format of the results files you want to publish.
    /// JUnit, NUnit, VSTest, XUnit, CTest
    /// Defaults to <code>JUnit</code>.
    /// </summary>
    [YamlIgnore]
    public TestResultsFormat TestResultsFormat
    {
        get => GetString("testResultsFormat") switch
        {
            "JUnit" => TestResultsFormat.JUnit,
            "NUnit" => TestResultsFormat.NUnit,
            "VSTest" => TestResultsFormat.VSTest,
            "XUnit" => TestResultsFormat.XUnit,
            "CTest" => TestResultsFormat.CTest,
            _ => throw new NotImplementedException(),
        };
        init => SetProperty("testResultsFormat", value switch {
            TestResultsFormat.JUnit => "JUnit",
            TestResultsFormat.NUnit => "NUnit",
            TestResultsFormat.VSTest => "VSTest",
            TestResultsFormat.XUnit => "XUnit",
            TestResultsFormat.CTest => "CTest",
            _ => throw new NotImplementedException() });
    }

    /// <summary>
    /// Specifies one or more test results files.
    /// You can use a single-folder wildcard (*) and recursive wildcards (**).
    /// Defaults to <code>**/TEST-*.xml</code>.
    /// <example>**/TEST-*.xml searches for all the XML files whose names start with TEST- in all subdirectories.</example>
    /// <remarks>If using VSTest as the test result format, the file type should be changed to .trx e.g. **/TEST-*.trx</remarks>
    /// </summary>
    [YamlIgnore]
    public string? TestResultsFiles
    {
        get => GetString("testResultsFiles");
        init => SetProperty("testResultsFiles", value);
    }

    /// <summary>
    /// Specifies the folder to search for the test result files.
    /// Defaults to <code>$(System.DefaultWorkingDirectory)</code>.
    /// </summary>
    [YamlIgnore]
    public string? SearchFolder
    {
        get => GetString("searchFolder") ?? "$(System.DefaultWorkingDirectory)";
        init => SetProperty("searchFolder", value);
    }

    /// <summary>
    /// When this boolean's value is true, the task reports test results from all the files against a single test run.
    /// If the value is false, the task creates a separate test run for each test result file.
    /// </summary>
    [YamlIgnore]
    public bool MergeTestResults
    {
        get => GetBool("mergeTestResults", false);
        init => SetProperty("mergeTestResults", value ? "true" : "false");
    }

    /// <summary>
    /// When this boolean's value is true, the task will fail if any of the tests in the results file are marked as failed.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnFailedTests
    {
        get => GetBool("failTaskOnFailedTests", false);
        init => SetProperty("failTaskOnFailedTests", value ? "true" : "false");
    }

    /// <summary>
    /// When true, fails the task if there is failure in publishing test results.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnFailureToPublishResults
    {
        get => GetBool("failTaskOnFailureToPublishResults", false);
        init => SetProperty("failTaskOnFailureToPublishResults", value ? "true" : "false");
    }

    /// <summary>
    /// Fail the task if no result files are found.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnMissingResultsFile
    {
        get => GetBool("failTaskOnMissingResultsFile", false);
        init => SetProperty("failTaskOnMissingResultsFile", value ? "true" : "false");
    }

    /// <summary>
    /// Specifies a name for the test run against which the results will be reported.
    /// Variable names declared in the build or release pipeline can be used.
    /// </summary>
    [YamlIgnore]
    public string? TestRunTitle
    {
        get => GetString("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Specifies the build platform against which the test run should be reported.
    /// <example>x64 or x86</example>
    /// </summary>
    [YamlIgnore]
    public string? BuildPlatform
    {
        get => GetString("buildPlatform");
        init => SetProperty("buildPlatform", value);
    }

    /// <summary>
    /// Specifies the build configuration against which the test run should be reported.
    /// <example>Debug or Release</example>
    /// </summary>
    [YamlIgnore]
    public string? BuildConfiguration
    {
        get => GetString("buildConfiguration");
        init => SetProperty("buildConfiguration", value);
    }

    /// <summary>
    /// When this boolean's value is true, the task uploads all the test result files as attachments to the test run.
    /// </summary>
    [YamlIgnore]
    public bool PublishRunAttachments
    {
        get => GetBool("publishRunAttachments", true);
        init => SetProperty("publishRunAttachments", value ? "true" : "false");
    }
}

public enum TestResultsFormat
{
    JUnit,
    NUnit,
    VSTest,
    XUnit,
    CTest
}
