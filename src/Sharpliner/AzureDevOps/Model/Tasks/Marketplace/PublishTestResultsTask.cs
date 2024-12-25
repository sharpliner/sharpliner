using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-test-results-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record PublishTestResultsTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublishTestResultsTask"/> class with required properties.
    /// </summary>
    /// <param name="testResultsFormat">The format of the results files you want to publish.</param>
    /// <param name="testResultsFile">The test results files.</param>
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
        get => GetEnum("testResultsFormat", TestResultsFormat.JUnit);
        init => SetProperty("testResultsFormat", value);
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
        init => SetProperty("mergeTestResults", value);
    }

    /// <summary>
    /// When this boolean's value is true, the task will fail if any of the tests in the results file are marked as failed.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnFailedTests
    {
        get => GetBool("failTaskOnFailedTests", false);
        init => SetProperty("failTaskOnFailedTests", value);
    }

    /// <summary>
    /// When true, fails the task if there is failure in publishing test results.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnFailureToPublishResults
    {
        get => GetBool("failTaskOnFailureToPublishResults", false);
        init => SetProperty("failTaskOnFailureToPublishResults", value);
    }

    /// <summary>
    /// Fail the task if no result files are found.
    /// </summary>
    [YamlIgnore]
    public bool FailTaskOnMissingResultsFile
    {
        get => GetBool("failTaskOnMissingResultsFile", false);
        init => SetProperty("failTaskOnMissingResultsFile", value);
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
        init => SetProperty("publishRunAttachments", value);
    }
}

/// <summary>
/// Test result formats supported by the PublishTestResults task.
/// </summary>
public enum TestResultsFormat
{
    /// <summary>
    /// The JUnit format, see <see href="https://github.com/windyroad/JUnit-Schema/blob/master/JUnit.xsd/">JUnit.xsd</see> for more details.
    /// </summary>
    [YamlMember(Alias = "JUnit")]
    JUnit,

    /// <summary>
    /// The NUnit format, see <see href="https://docs.nunit.org/articles/nunit/technical-notes/usage/Test-Result-XML-Format.html>">Test Result XML Format</see> for more details.
    /// </summary>
    [YamlMember(Alias = "NUnit")]
    NUnit,

    /// <summary>
    /// This is the XML report format of the Microsoft's unit test framework. A XSD schema of it can be found at your Visual Studio's installation directory - <c>%VSINSTALLDIR%\xml\Schemas\vstst.xsd</c>.
    /// </summary>
    [YamlMember(Alias = "VSTest")]
    VSTest,

    /// <summary>
    /// The xunit format, see <see href="https://xunit.net/docs/format-xml-v2">xUnit.net v2+ XML Format</see> for more details.
    /// </summary>
    [YamlMember(Alias = "XUnit")]
    XUnit,

    /// <summary>
    /// The CTest format, see <see href="https://cmake.org/cmake/help/latest/manual/ctest.1.html#introduction">CTest</see> for more details.
    /// </summary>
    [YamlMember(Alias = "CTest")]
    CTest
}
