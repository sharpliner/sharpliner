namespace Sharpliner.AzureDevOps;

/// <summary>
/// No-op job is used when other jobs might be omitted based on a condition and no job is left in the stage.
/// </summary>
public record NoopJob : Job
{
    public NoopJob() : base("No_op")
    {
    }
}
