using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>SSH@0</c> tasks.
/// </summary>
public class SshTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates an SSH task that runs one or more commands on the remote machine.
    /// Emits <c>runOptions: commands</c>.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="commands">Shell commands to run on the remote machine.</param>
    /// <returns>A new instance of <see cref="SshCommandsTask"/>.</returns>
    public SshCommandsTask Commands(AdoExpression<string> sshEndpoint, params string[] commands)
        => new(sshEndpoint, string.Join("\n", commands));

    /// <summary>
    /// Creates an SSH task that runs a script file on the remote machine.
    /// Emits <c>runOptions: script</c>.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="scriptPath">Path to the shell script file to run on the remote machine.</param>
    /// <param name="args">Optional arguments passed to the shell script.</param>
    /// <returns>A new instance of <see cref="SshScriptTask"/>.</returns>
    public SshScriptTask Script(AdoExpression<string> sshEndpoint, AdoExpression<string> scriptPath, AdoExpression<string>? args = null)
    {
        var task = new SshScriptTask(sshEndpoint, scriptPath);

        if (args is not null)
        {
            task = task with
            {
                Args = args,
            };
        }

        return task;
    }

    /// <summary>
    /// Creates an SSH task that runs inline script contents on the remote machine.
    /// Emits <c>runOptions: inline</c>.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="scriptLines">Inline script lines to run on the remote machine.</param>
    /// <returns>A new instance of <see cref="SshInlineTask"/>.</returns>
    public SshInlineTask Inline(AdoExpression<string> sshEndpoint, params string[] scriptLines)
        => new(sshEndpoint, string.Join("\n", scriptLines));

    /// <summary>
    /// Creates an SSH inline-script task from an embedded resource file.
    /// Emits <c>runOptions: inline</c>.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="resourceFileName">Name of the embedded resource file.</param>
    /// <returns>A new instance of <see cref="SshInlineTask"/>.</returns>
    public SshInlineTask FromResourceFile(AdoExpression<string> sshEndpoint, string resourceFileName)
        => new(sshEndpoint, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName));

    /// <summary>
    /// Creates an SSH inline-script task from a local file.
    /// Emits <c>runOptions: inline</c>.
    /// </summary>
    /// <param name="sshEndpoint">SSH service connection with connection details for the remote machine.</param>
    /// <param name="path">Path to the local file that contains script contents.</param>
    /// <returns>A new instance of <see cref="SshInlineTask"/>.</returns>
    public SshInlineTask FromFile(AdoExpression<string> sshEndpoint, string path)
        => new(sshEndpoint, System.IO.File.ReadAllText(path));

    internal SshTaskBuilder()
    {
    }
}
