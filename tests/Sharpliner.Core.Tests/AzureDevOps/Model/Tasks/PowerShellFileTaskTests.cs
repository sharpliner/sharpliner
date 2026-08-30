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
            ErrorActionPreference = ActionPreference.Continue,
            WarningPreference = ActionPreference.Stop,
            InformationPreference = ActionPreference.SilentlyContinue,
            DebugPreference = ActionPreference.Default,
            VerbosePreference = ActionPreference.Continue,
            ProgressPreference = ActionPreference.Default,
            FailOnStderr = true,
            ShowWarnings = true,
            IgnoreLASTEXITCODE = true,
            WorkingDirectory = "some/dir",
            RunScriptInSeparateScope = true,
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
    public Task Serialize_Pwsh_File_Task_Test()
    {
        var task = new PowershellFileTask("some\\script.ps1", true)
        {
            Arguments = "-Foo bar",
            ProgressPreference = ActionPreference.SilentlyContinue,
            RunScriptInSeparateScope = true,
            DisplayName = "Test task"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Powershell_Step_Test()
    {
        var task = new InlinePowershellTask("Write-Output 'Hello'", "Write-Output 'World'")
        {
            DisplayName = "Test task",
            WorkingDirectory = "some/dir",
            ErrorActionPreference = ActionPreference.Continue,
            FailOnStderr = true,
            IgnoreLASTEXITCODE = true,
        };

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
