using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class PowerShellFileTaskTests
{
    [Fact]
    public Task Serialize_Powershell_File_Task_Test()
    {
        var task = new PowershellFileTask("some\\script.ps1", false)
        {
            Arguments = "foo bar",
            ContinueOnError = true,
            ErrorActionPreference = ActionPreference.Inquire,
            WarningPreference = ActionPreference.Stop,
            InformationPreference = ActionPreference.Break,
            DebugPreference = ActionPreference.Suspend,
            VerbosePreference = ActionPreference.Break,
            FailOnStderr = true,
            IgnoreLASTEXITCODE = true,
            DisplayName = "Test task"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Powershell_File_Task_With_Defaults_Test()
    {
        var task = new PowershellFileTask("some\\script.ps1", true).DisplayAs("Test task");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Pwsh_Step_Test()
    {
        var task = new InlinePwshTask("Write-Output 'Hello'", "Write-Output 'World'")
        {
            DisplayName = "Test task",
            ContinueOnError = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
