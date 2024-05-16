using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PowerShellFileTaskTests
{
    [Fact]
    public void Serialize_Powershell_File_Task_Test()
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

        string yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be(
            """
            task: PowerShell@2

            displayName: Test task

            inputs:
              targetType: filePath
              filePath: some\script.ps1
              arguments: foo bar
              errorActionPreference: Inquire
              warningPreference: Stop
              informationPreference: Break
              verbosePreference: Break
              debugPreference: Suspend
              failOnStderr: true
              ignoreLASTEXITCODE: true

            continueOnError: true
            """);
    }

    [Fact]
    public void Serialize_Powershell_File_Task_With_Defaults_Test()
    {
        var task = new PowershellFileTask("some\\script.ps1", true).DisplayAs("Test task");

        string yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be(
            """
            task: PowerShell@2

            displayName: Test task

            inputs:
              targetType: filePath
              filePath: some\script.ps1
              pwsh: true
            """);
    }

    [Fact]
    public void Serialize_Pwsh_Step_Test()
    {
        var task = new InlinePwshTask("Write-Output 'Hello'", "Write-Output 'World'")
        {
            DisplayName = "Test task",
            ContinueOnError = true,
        };

        string yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be(
            """
            pwsh: |-
              Write-Output 'Hello'
              Write-Output 'World'

            displayName: Test task

            continueOnError: true
            """);
    }
}
