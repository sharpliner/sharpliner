namespace Sharpliner.AzureDevOps.Tasks;

public class DownloadTaskBuilder
{
    /// <summary>
    /// Creates a download task that downloads artifacts from the current build.
    /// </summary>
    public CurrentDownloadTask Current => new();

    /// <summary>
    /// Creates a download task that skips downloading artifacts for the current job.
    /// </summary>
    public NoneDownloadTask None => new();

    /// <summary>
    /// Creates a download task that an artifact from a given pipeline run.
    /// </summary>
    public SpecificDownloadTask SpecificBuild(string project, int definition) => new(project, definition);

    internal DownloadTaskBuilder()
    {
    }
}
