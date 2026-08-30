using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Defines the <c>CopyFiles@2</c> Azure Pipelines task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/copy-files-v2">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/CopyFilesV2/task.json">official task specification</see>.
/// </summary>
public record CopyFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// The source folder that the copy patterns run from.
    /// If the folder is empty, the task copies files from the root folder of the repo as though <c>$(Build.SourcesDirectory)</c> was specified.
    /// Defaults to an empty string.
    ///
    /// <remarks>If your build produces artifacts outside of the sources directory, specify <c>$(Agent.BuildDirectory)</c> to copy files from the directory created for the pipeline.</remarks>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SourceFolder
    {
        get => GetExpression<string>("SourceFolder");
        init => SetProperty("SourceFolder", value);
    }

    /// <summary>
    /// File paths to include as part of the copy. Supports multiple lines of match patterns.
    /// Defaults to <code>**</code>.
    ///
    /// <remarks>The patterns match only file paths, not folder paths. Specify patterns, such as <c>**\bin\**</c> instead of <c>**\bin</c>.</remarks>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Contents
    {
        get => GetExpression<string>("Contents");
        init => SetProperty("Contents", value);
    }

    /// <summary>
    /// The target folder or UNC path that will contain the copied files. You can use variables.
    /// Defaults to an empty string.
    ///
    /// <example>$(Build.ArtifactStagingDirectory)</example>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TargetFolder
    {
        get => GetExpression<string>("TargetFolder");
        init => SetProperty("TargetFolder", value);
    }

    /// <summary>
    /// Deletes all existing files in the target folder before the copy process.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CleanTargetFolder
    {
        get => GetExpression<bool>("CleanTargetFolder");
        init => SetProperty("CleanTargetFolder", value);
    }

    /// <summary>
    /// Replaces the existing files in the target folder.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Overwrite
    {
        get => GetExpression<bool>("OverWrite");
        init => SetProperty("OverWrite", value);
    }

    /// <summary>
    /// Flattens the folder structure and copies all files into the specified target folder.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FlattenFolders
    {
        get => GetExpression<bool>("flattenFolders");
        init => SetProperty("flattenFolders", value);
    }

    /// <summary>
    /// Preserves the target file timestamp by using the original source file.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PreserveTimestamp
    {
        get => GetExpression<bool>("preserveTimestamp");
        init => SetProperty("preserveTimestamp", value);
    }

    /// <summary>
    /// Specifies the retry count to copy the file. It might help resolve intermittent issues, for example with UNC target paths on a remote host.
    /// Defaults to <code>0</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RetryCount
    {
        get => GetExpression<string>("retryCount");
        init => SetProperty("retryCount", value);
    }

    /// <summary>
    /// Specifies the delay between two retries. It might help make the copy more resilient to intermittent issues, for example with UNC target paths on a remote host.
    /// Defaults to <code>1000</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DelayBetweenRetries
    {
        get => GetExpression<string>("delayBetweenRetries");
        init => SetProperty("delayBetweenRetries", value);
    }

    /// <summary>
    /// Ignores errors that occur during the creation of the target folder.
    /// This string is useful for avoiding issues with the parallel execution of tasks by several agents within one target folder.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IgnoreMakeDirErrors
    {
        get => GetExpression<bool>("ignoreMakeDirErrors");
        init => SetProperty("ignoreMakeDirErrors", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CopyFilesTask"/> class with required properties.
    /// </summary>
    /// <param name="contents">The file paths to include as part of the copy.</param>
    /// <param name="targetFolder">The target folder or UNC path that will contain the copied files.</param>
    public CopyFilesTask(string contents, string targetFolder) : base("CopyFiles@2")
    {
        Contents = contents;
        TargetFolder = targetFolder;
    }
}
