using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// The Behavior of the lock for the stage. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/approvals#exclusive-lock">Exclusive lock</see> for more details.
/// </summary>
public enum LockBehavior
{
    /// <summary>
    /// Only the latest run acquires the lock to the resource. this is the default if no behavior is specified.
    /// </summary>
    [YamlMember(Alias = "runLatest")]
    RunLatest,

    /// <summary>
    /// All runs acquire the lock to the protected resource sequentially.
    /// </summary>
    [YamlMember(Alias = "sequential")]
    Sequential
}
