﻿using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/delete-files-v1">official Azure DevOps pipelines documentation</see>
/// </summary>
public record DeleteFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the folder to delete files from.
    /// If the source folder is empty, the task deletes files from the root folder of the repository as though $(Build.SourcesDirectory) was specified.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SourceFolder
    {
        get => GetExpression<string>("SourceFolder");
        init => SetProperty("SourceFolder", value);
    }

    /// <summary>
    /// The file/folder paths to delete.
    /// Defaults to <code>myFileShare</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Contents
    {
        get => GetExpression<string>("Contents");
        init => SetProperty("Contents", value);
    }

    /// <summary>
    /// Attempts to remove the source folder after attempting to remove Contents. If you want to remove the whole folder, set this to true and set Contents to *.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RemoveSourceFolder
    {
        get => GetExpression<bool>("RemoveSourceFolder");
        init => SetProperty("RemoveSourceFolder", value);
    }

    /// <summary>
    /// Deletes files starting with a dot.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RemoveDotFiles
    {
        get => GetExpression<bool>("RemoveDotFiles");
        init => SetProperty("RemoveDotFiles", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFilesTask"/> class with required properties.
    /// </summary>
    /// <param name="contents">The file/folder paths to delete.</param>
    public DeleteFilesTask(string contents) : base("DeleteFiles@1")
    {
        Contents = contents;
    }
}
