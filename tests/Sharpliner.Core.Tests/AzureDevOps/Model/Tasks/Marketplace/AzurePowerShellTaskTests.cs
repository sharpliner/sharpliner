using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzurePowerShellTaskTests
{
    [Fact]
    public Task Serialize_InlineTask_Test()
    {
        var task = new InlineAzurePowerShellTask("connectedServiceNameARM", "Get-AzResourceGroup")
        {
            ErrorActionPreference = PowerShellErrorActionPreference.SilentlyContinue,
            AzurePowerShellVersion = AzurePowerShellVersion.LatestVersion,
            Pwsh = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_InlineTask_With_Defaults_Test()
    {
        var task = new InlineAzurePowerShellTask("connectedServiceNameARM", "Get-AzResourceGroup");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FileTask_Test()
    {
        var task = new AzurePowerShellFileTask("connectedServiceNameARM", "foo.ps1")
        {
            ScriptArguments = "-Name value",
            ValidateScriptSignature = true,
            FailOnStandardError = true,
            WorkingDirectory = "src",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FileTask_With_Defaults_Test()
    {
        var task = new AzurePowerShellFileTask("connectedServiceNameARM", "foo.ps1");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_FileTask_With_OtherVersion_Test()
    {
        var task = new AzurePowerShellFileTask("connectedServiceNameARM", "foo.ps1")
        {
            AzurePowerShellVersion = AzurePowerShellVersion.OtherVersion,
            PreferredAzurePowerShellVersion = "4.1.0",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V4_InlineTask_Test()
    {
        var task = new InlineAzurePowerShellV4Task("connectedServiceNameARM", "Get-AzResourceGroup")
        {
            RestrictContextToCurrentTask = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_V4_FileTask_Test()
    {
        var task = new AzurePowerShellV4FileTask("connectedServiceNameARM", "foo.ps1")
        {
            RestrictContextToCurrentTask = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
