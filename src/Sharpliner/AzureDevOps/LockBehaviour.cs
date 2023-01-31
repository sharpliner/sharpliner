using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

public enum LockBehaviour
{
    [YamlMember(Alias = "runLatest")]
    RunLatest,

    [YamlMember(Alias = "sequential")]
    Sequential
}
