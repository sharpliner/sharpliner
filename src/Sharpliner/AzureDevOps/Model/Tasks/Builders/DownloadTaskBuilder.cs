namespace Sharpliner.AzureDevOps.Tasks;

public class DownloadTaskBuilder
{
    public CurrentDownloadTask Current => new();

    public NoneDownloadTask None => new();

    /// <param name="pipelineResourceIdentifier">The definition ID of the build pipeline.</param>
    public SpecificDownloadTask SpecificBuild(string pipelineResourceIdentifier) => new(pipelineResourceIdentifier);

    internal DownloadTaskBuilder()
    {
    }
}
