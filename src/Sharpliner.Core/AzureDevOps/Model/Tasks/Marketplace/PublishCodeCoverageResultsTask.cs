using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Publishes code coverage results using the <c>PublishCodeCoverageResults@2</c> Azure DevOps task.
/// The supported specification is <c>PublishCodeCoverageResultsV2</c> version <c>2.279.0</c>, retrieved from
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/PublishCodeCoverageResultsV2/task.json">the official task.json</see>
/// on 2026-08-30. Version 2 is the current supported major version; version 1 is deprecated.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-code-coverage-results-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PublishCodeCoverageResultsTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the paths to summary files containing code coverage statistics, such as line, method, and class coverage.
    /// This required <c>multiLine</c> input supports one or more minimatch patterns separated by new lines.
    /// Multiple summary files are merged into a single report and the task auto-generates the HTML coverage report.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SummaryFileLocation
    {
        get => GetExpression<string>("summaryFileLocation");
        init => SetProperty("summaryFileLocation", value);
    }

    /// <summary>
    /// Specifies the optional path to source files. This is required when coverage XML reports do not contain absolute paths,
    /// such as JaCoCo reports, or when tests run in a Docker container and the task must map container paths to host source paths.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PathToSources
    {
        get => GetExpression<string>("pathToSources");
        init => SetProperty("pathToSources", value);
    }

    /// <summary>
    /// Fails the task if the summary file patterns do not produce any code coverage results to publish.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailIfCoverageEmpty
    {
        get => GetExpression<bool>("failIfCoverageEmpty");
        init => SetProperty("failIfCoverageEmpty", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishCodeCoverageResultsTask"/> class with required properties.
    /// </summary>
    /// <param name="summaryFileLocation">
    /// The paths to summary files containing code coverage statistics. Use new-line separated minimatch patterns for multiple files.
    /// </param>
    public PublishCodeCoverageResultsTask(string summaryFileLocation) : base("PublishCodeCoverageResults@2")
    {
        DisplayName = "Publish code coverage results";
        SummaryFileLocation = summaryFileLocation;
    }
}
