using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

public class BashTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates a bash task where the contents come from an embedded resource.
    /// </summary>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    public InlineBashTask FromResourceFile(string resourceFileName, string? displayName = null!)
        => new InlineBashTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
        };

    /// <summary>
    /// Creates a bash task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    public InlineBashTask FromFile(string path, string? displayName = null!) => new InlineBashTask(System.IO.File.ReadAllText(path)) with
    {
        DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
    };

    /// <summary>
    /// Creates a bash task referencing a bash file (contents are not inlined in the YAML).
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="displayName">Name of the build step</param>
    public BashFileTask File(string filePath, string? displayName = null) => new BashFileTask(filePath)
    {
        DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
    };

    /// <summary>
    /// Creates a bash task with given contents.
    /// </summary>
    /// <param name="scriptLines">Contents of the script</param>
    public InlineBashTask Inline(params string[] scriptLines) => new(scriptLines);

    internal BashTaskBuilder()
    {
    }
}
