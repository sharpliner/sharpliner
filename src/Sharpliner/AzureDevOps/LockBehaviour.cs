using System.Diagnostics.CodeAnalysis;

namespace Sharpliner.AzureDevOps;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum LockBehaviour
{
    runLatest,
    sequential
}
