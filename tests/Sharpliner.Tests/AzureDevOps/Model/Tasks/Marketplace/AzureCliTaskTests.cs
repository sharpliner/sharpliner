using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureCliTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new AzureCliTask("connectedServiceNameARM", ScriptType.Ps, ScriptLocation.InlineScript)
        {
            InlineScript = "Write-Host \"test\"",
            Arguments = "arg1 arg2",
            PowerShellErrorActionPreference = PowerShellErrorActionPreference.SilentlyContinue
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new AzureCliTask("connectedServiceNameARM", ScriptType.Ps, ScriptLocation.InlineScript)
        {
            InlineScript = "Write-Host \"test\"",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
