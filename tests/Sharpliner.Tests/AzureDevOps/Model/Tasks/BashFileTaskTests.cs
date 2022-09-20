using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class BashFileTaskTests
{
    [Fact]
    public void Serialize_Bash_File_Task_Test()
    {
        var task = new BashFileTask("some/script.sh")
        {
            Arguments = "foo bar",
            ContinueOnError = true,
            FailOnStderr = true,
            BashEnv = "~/.bash_profile",
            DisplayName = "Test task"
        };

        string yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be(
            """
            task: Bash@3

            displayName: Test task

            inputs:
              targetType: filePath
              filePath: some/script.sh
              arguments: foo bar
              failOnStderr: true
              bashEnvValue: ~/.bash_profile

            continueOnError: true
            """);
    }

    [Fact]
    public void Serialize_Bash_File_Task_With_Defaults_Test()
    {
        var task = new BashFileTask("some/script.sh").DisplayAs("Test task");

        string yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be(
            """
            task: Bash@3

            displayName: Test task

            inputs:
              targetType: filePath
              filePath: some/script.sh
            """);
    }
}
