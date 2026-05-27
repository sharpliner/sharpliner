using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureCliTaskTests
{
    [Fact]
    public Task Serialize_InlineTask_Test()
    {
        var task = new InlineAzureCliTask("connectedServiceNameARM", ScriptType.Ps, "Write-Host \"test\"")
        {
            Arguments = "arg1 arg2",
            PowerShellErrorActionPreference = PowerShellErrorActionPreference.SilentlyContinue
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_InlineTask_With_Defaults_Test()
    {
        var task = new InlineAzureCliTask("connectedServiceNameARM", ScriptType.Ps, "Write-Host \"test\"")
        {
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FileTask_Test()
    {
        var task = new AzureCliFileTask("connectedServiceNameARM", ScriptType.Ps, "foo.ps1")
        {
            Arguments = "arg1 arg2",
            PowerShellErrorActionPreference = PowerShellErrorActionPreference.SilentlyContinue
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FileTask_With_Defaults_Test()
    {
        var task = new AzureCliFileTask("connectedServiceNameARM", ScriptType.Ps, "foo.ps1")
        {
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
