using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

public class PowershellTaskBuilder : TaskBuilderBase
{
    private readonly bool _pwsh;

    /// <summary>
    /// Creates a Powershell task where the contents come from an embedded resource.
    /// </summary>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    public InlinePowershellTask FromResourceFile(string resourceFileName, string? displayName = null)
        => new InlinePowershellTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
            Pwsh = _pwsh
        };

    /// <summary>
    /// Creates a Powershell task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    public InlinePowershellTask FromFile(string path, string? displayName = null)
        => new InlinePowershellTask(System.IO.File.ReadAllText(path)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
            Pwsh = _pwsh
        };

    /// <summary>
    /// Creates a Powershell task referencing a Powershell file (contents are not inlined in the YAML).
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    public PowershellFileTask File(string filePath, string? displayName = null)
        => new PowershellFileTask(filePath) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
            Pwsh = _pwsh
        };

    /// <summary>
    /// Creates a Powershell task with given contents.
    /// </summary>
    /// <param name="scriptLines">Contents of the script</param>
    public InlinePowershellTask Inline(params string[] scriptLines)
        => new InlinePowershellTask(scriptLines) with
        {
            Pwsh = _pwsh
        };

    internal PowershellTaskBuilder(bool pwsh)
    {
        _pwsh = pwsh;
    }
}
