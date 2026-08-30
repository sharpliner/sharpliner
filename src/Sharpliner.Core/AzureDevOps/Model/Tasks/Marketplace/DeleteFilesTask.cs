using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/delete-files-v1">official Azure DevOps pipelines documentation</see>.
/// The official task specification is available in the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DeleteFilesV1/task.json">Azure Pipelines Tasks repository</see>.
/// </summary>
public record DeleteFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the folder that the deletions are run from.
    /// If the source folder is empty, the task deletes files from the root folder of the repository as though <c>$(Build.SourcesDirectory)</c> was specified.
    /// Use variables such as <c>$(Agent.BuildDirectory)</c> if the files are outside the repository.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SourceFolder
    {
        get => GetExpression<string>("SourceFolder");
        init => SetProperty("SourceFolder", value);
    }

    /// <summary>
    /// The file or folder paths to delete.
    /// Supports multiple lines of minimatch patterns.
    /// Defaults to <c>myFileShare</c> in the official task.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Contents
    {
        get => GetExpression<string>("Contents");
        init => SetProperty("Contents", value);
    }

    /// <summary>
    /// Attempts to remove the source folder after attempting to remove <see cref="Contents"/>.
    /// If you want to remove the whole folder, set this to <c>true</c> and set <see cref="Contents"/> to <c>*</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RemoveSourceFolder
    {
        get => GetExpression<bool>("RemoveSourceFolder");
        init => SetProperty("RemoveSourceFolder", value);
    }

    /// <summary>
    /// Deletes files starting with a dot, such as <c>.git</c> or <c>.dockerfile</c>.
    /// Otherwise, dot files are omitted unless they are specified explicitly with a pattern such as <c>/.*</c>.
    /// See <see href="https://github.com/isaacs/minimatch#dot">minimatch dot matching</see> for more information.
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
