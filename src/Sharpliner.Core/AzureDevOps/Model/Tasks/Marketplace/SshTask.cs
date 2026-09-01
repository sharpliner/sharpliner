using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs shell commands or a script on a remote machine using the <c>SSH@0</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/ssh-v0?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/SshV0/task.json">official SshV0 task specification</see>.
/// </summary>
public abstract record SshTask : AzureDevOpsTask
{
    /// <summary>
    /// Required. SSH service connection with connection details for the remote machine.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SshEndpoint
    {
        get => GetExpression<string>("sshEndpoint");
        init => SetProperty("sshEndpoint", value);
    }

    /// <summary>
    /// Required. Mode used to run on the remote machine: <c>commands</c>, <c>script</c>, or <c>inline</c>.
    /// Default value: <c>commands</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<SshRunOptions>? RunOptions
    {
        get => GetExpression<SshRunOptions>("runOptions", SshRunOptions.Commands);
        init => SetProperty("runOptions", value);
    }

    /// <summary>
    /// Optional. Default value: <c>true</c>. If true, the task fails when remote output writes to <c>STDERR</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnStdErr
    {
        get => GetExpression<bool>("failOnStdErr", true);
        init => SetProperty("failOnStdErr", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. Starts an interactive SSH session.
    /// Useful for commands such as <c>sudo</c> that can prompt for a password.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? InteractiveSession
    {
        get => GetExpression<bool>("interactiveSession", false);
        init => SetProperty("interactiveSession", value);
    }

    /// <summary>
    /// Required. SSH handshake timeout in milliseconds.
    /// Default value: <c>20000</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ReadyTimeout
    {
        get => GetExpression<string>("readyTimeout", "20000");
        init => SetProperty("readyTimeout", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. Enables interactive-keyboard authentication.
    /// Use this when password authentication is disabled on the target machine.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? InteractiveKeyboardAuthentication
    {
        get => GetExpression<bool>("interactiveKeyboardAuthentication", false);
        init => SetProperty("interactiveKeyboardAuthentication", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. When false, remote <c>##vso[...]</c> output is printed as text and not executed as logging commands.
    /// Enable only when remote output is trusted and you intentionally depend on remote logging commands.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? EnableRemoteVsoCommands
    {
        get => GetExpression<bool>("enableRemoteVsoCommands", false);
        init => SetProperty("enableRemoteVsoCommands", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshTask"/> class with required properties.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="runOptions">Mode used to run on the remote machine.</param>
    protected SshTask(AdoExpression<string> sshEndpoint, AdoExpression<SshRunOptions> runOptions)
        : base("SSH@0")
    {
        SshEndpoint = sshEndpoint;
        RunOptions = runOptions;
    }
}

/// <summary>
/// SSH task that runs shell commands on the remote machine using <c>runOptions: commands</c>.
/// </summary>
public record SshCommandsTask : SshTask
{
    /// <summary>
    /// Required when <c>runOptions = commands</c>. Shell commands to run on the remote machine.
    /// Enter each command with its arguments on a new line.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Commands
    {
        get => GetExpression<string>("commands");
        init => SetProperty("commands", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshCommandsTask"/> class.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="commands">Shell commands to run on the remote machine.</param>
    public SshCommandsTask(AdoExpression<string> sshEndpoint, AdoExpression<string> commands)
        : base(sshEndpoint, SshRunOptions.Commands)
    {
        Commands = commands;
    }
}

/// <summary>
/// SSH task that runs a shell script file on the remote machine using <c>runOptions: script</c>.
/// </summary>
public record SshScriptTask : SshTask
{
    /// <summary>
    /// Required when <c>runOptions = script</c>. Path to the shell script file to run on the remote machine.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptPath
    {
        get => GetExpression<string>("scriptPath");
        init => SetProperty("scriptPath", value);
    }

    /// <summary>
    /// Optional. Use when <c>runOptions = script</c>. Arguments passed to the shell script.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Args
    {
        get => GetExpression<string>("args");
        init => SetProperty("args", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshScriptTask"/> class.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="scriptPath">Path to the shell script file to run on the remote machine.</param>
    public SshScriptTask(AdoExpression<string> sshEndpoint, AdoExpression<string> scriptPath)
        : base(sshEndpoint, SshRunOptions.Script)
    {
        ScriptPath = scriptPath;
    }
}

/// <summary>
/// SSH task that runs an inline shell script on the remote machine using <c>runOptions: inline</c>.
/// </summary>
public record SshInlineTask : SshTask
{
    /// <summary>
    /// Required when <c>runOptions = inline</c>. Inline shell script to run on the remote machine.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Inline
    {
        get => GetExpression<string>("inline");
        init => SetProperty("inline", value);
    }

    /// <summary>
    /// Optional. Use when <c>runOptions = inline</c>.
    /// Path to the command interpreter used to execute the script. Default value: <c>/bin/bash</c>.
    /// Use an empty string for Windows-based remote hosts.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? InterpreterCommand
    {
        get => GetExpression<string>("interpreterCommand", "/bin/bash");
        init => SetProperty("interpreterCommand", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SshInlineTask"/> class.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="inline">Inline shell script to run on the remote machine.</param>
    public SshInlineTask(AdoExpression<string> sshEndpoint, AdoExpression<string> inline)
        : base(sshEndpoint, SshRunOptions.Inline)
    {
        Inline = inline;
    }
}

/// <summary>
/// Allowed values for the SSH task <c>runOptions</c> input.
/// </summary>
public enum SshRunOptions
{
    /// <summary>
    /// Run shell commands.
    /// </summary>
    [YamlMember(Alias = "commands")]
    Commands,

    /// <summary>
    /// Run a shell script file.
    /// </summary>
    [YamlMember(Alias = "script")]
    Script,

    /// <summary>
    /// Run an inline shell script.
    /// </summary>
    [YamlMember(Alias = "inline")]
    Inline,
}
