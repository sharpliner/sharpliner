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
    public string? SourceFolder
    {
        get => GetString("SourceFolder");
        init => SetProperty("SourceFolder", value);
    }

    /// <summary>
    /// The file/folder paths to delete.
    /// Defaults to <code>myFileShare</code>.
    /// </summary>
    [YamlIgnore]
    public string? Contents
    {
        get => GetString("Contents");
        init => SetProperty("Contents", value);
    }

    /// <summary>
    /// Attempts to remove the source folder after attempting to remove Contents. If you want to remove the whole folder, set this to true and set Contents to *.
    /// </summary>
    [YamlIgnore]
    public bool RemoveSourceFolder
    {
        get => GetBool("RemoveSourceFolder", false);
        init => SetProperty("RemoveSourceFolder", value);
    }

    /// <summary>
    /// Deletes files starting with a dot.
    /// </summary>
    [YamlIgnore]
    public bool RemoveDotFiles
    {
        get => GetBool("RemoveDotFiles", false);
        init => SetProperty("RemoveDotFiles", value);
    }

    public DeleteFilesTask(string contents) : base("DeleteFiles@1")
    {
        Contents = contents;
    }
}
