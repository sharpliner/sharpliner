using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class SshTaskTests
{
    [Fact]
    public Task Serialize_CommandsTask_Test()
    {
        var task = new SshCommandsTask("my-ssh-endpoint", "cd /var/www\n./deploy.sh")
        {
            InteractiveSession = true,
            ReadyTimeout = "30000"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_ScriptTask_Test()
    {
        var task = new SshScriptTask("my-ssh-endpoint", "scripts/deploy.sh")
        {
            Args = "--environment prod --verbose",
            FailOnStdErr = false,
            InteractiveKeyboardAuthentication = true,
            EnableRemoteVsoCommands = true
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_InlineTask_Test()
    {
        var task = new SshInlineTask("my-ssh-endpoint", "set -euo pipefail\n./deploy.sh")
        {
            InterpreterCommand = "/bin/sh",
            InteractiveSession = true
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Tasks_With_Defaults_Test()
    {
        var tasks = new object[]
        {
            new SshCommandsTask("my-ssh-endpoint", "echo hello"),
            new SshScriptTask("my-ssh-endpoint", "scripts/deploy.sh"),
            new SshInlineTask("my-ssh-endpoint", "echo hello")
        };

        return Verify(SharplinerSerializer.Serialize(tasks));
    }
}
