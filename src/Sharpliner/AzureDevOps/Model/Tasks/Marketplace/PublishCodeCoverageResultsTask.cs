using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-code-coverage-results-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record PublishCodeCoverageResultsTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the path of the summary file containing code coverage statistics, such as line, method, and class coverage.
    /// Multiple summary files are merged into a single report.
    /// </summary>
    [YamlIgnore]
    public string? SummaryFileLocation
    {
        get => GetString("summaryFileLocation");
        init => SetProperty("summaryFileLocation", value);
    }

    /// <summary>
    /// Specifying a path to source files is required when coverage XML reports don't contain an absolute path to source files.
    /// </summary>
    [YamlIgnore]
    public string? PathToSources
    {
        get => GetString("pathToSources");
        init => SetProperty("pathToSources", value);
    }

    /// <summary>
    /// Fails the task if code coverage did not produce any results to publish.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public bool FailIfCoverageEmpty
    {
        get => GetBool("failIfCoverageEmpty", false);
        init => SetProperty("failIfCoverageEmpty", value ? "true" : "false");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishCodeCoverageResultsTask"/> class with required properties.
    /// </summary>
    /// <param name="summaryFileLocation">The path of the summary file containing code coverage statistics.</param>
    public PublishCodeCoverageResultsTask(string summaryFileLocation) : base("PublishCodeCoverageResults@2")
    {
        SummaryFileLocation = summaryFileLocation;
    }
}
