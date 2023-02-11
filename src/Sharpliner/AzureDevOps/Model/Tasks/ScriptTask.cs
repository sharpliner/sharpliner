using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/command-line?view=azure-devops&amp;tabs=yaml">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record ScriptTask : Step
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
    /// </summary>
    [YamlMember(Alias = "script", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public string Contents { get; }

    /// <summary>
    /// Specify the working directory in which you want to run the command.
    /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
    /// </summary>
    [YamlMember(Order = 113)]
    public Conditioned<string>? WorkingDirectory { get; init; }

    /// <summary>
    /// If this is true, this task will fail if any errors are written to stderr.
    /// Defaults to 'false'.
    /// </summary>
    [YamlMember(Order = 200)]
    public Conditioned<bool>? FailOnStdErr { get; init; }

    public ScriptTask(params string[] scriptLines)
    {
        if (scriptLines is null)
        {
            throw new ArgumentNullException(nameof(scriptLines));
        }

        Contents = string.Join("\r\n", scriptLines);
    }
}
