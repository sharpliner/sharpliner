using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/copy-files-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record CopyFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// The folder that contains the files you want to copy.
    /// If the folder is empty, then the task copies files from the root folder of the repo as though $(Build.SourcesDirectory) was specified.
    ///
    /// <remarks>If your build produces artifacts outside of the sources directory, specify $(Agent.BuildDirectory) to copy files from the directory created for the pipeline.</remarks>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SourceFolder
    {
        get => GetExpression<string>("SourceFolder");
        init => SetProperty("SourceFolder", value);
    }

    /// <summary>
    /// The file paths to include as part of the copy.
    /// Defaults to <code>**</code>.
    ///
    /// <remarks>The pattern is used to match only file paths, not folder paths. Specify patterns, such as **\bin\** instead of **\bin.</remarks>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Contents
    {
        get => GetExpression<string>("Contents");
        init => SetProperty("Contents", value);
    }

    /// <summary>
    /// The target folder or UNC path that will contain the copied files. You can use variables.
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
        get => GetExpression<bool>("Overwrite");
        init => SetProperty("Overwrite", value);
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
    /// Specifies the retry count to copy the file.
    /// Defaults to <code>0</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RetryCount
    {
        get => GetExpression<int>("retryCount");
        init => SetProperty("retryCount", value);
    }

    /// <summary>
    /// Specifies the delay between two retries.
    /// Defaults to <code>1000</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? DelayBetweenRetries
    {
        get => GetExpression<int>("delayBetweenRetries");
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
