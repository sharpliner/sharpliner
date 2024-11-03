using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a script task using the <c>script</c> keyword.
/// </summary>
public class ScriptTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// <para>
    /// Creates a script task where the contents come from an embedded resource.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// assuming the embedded resource file <c>my-script</c> contains:
    /// <code>
    /// echo "Hello, world!"
    /// echo "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// and with the code:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Script.FromResourceFile("my-script", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - script: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'Run a script'
    /// </code>
    /// </summary>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    public ScriptTask FromResourceFile(string resourceFileName, string? displayName = null)
        => new ScriptTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
        };

    /// <summary>
    /// <para>
    /// Creates a script task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// assuming the file <c>AzureDevOps/Resources/script</c> contains:
    /// <code>
    /// echo "Hello, world!"
    /// echo "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// and with the code:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Script.FromFile("AzureDevOps/Resources/my-script", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - script: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'Run a script'
    /// </code>
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    public ScriptTask FromFile(string path, string? displayName = null)
        => new ScriptTask(System.IO.File.ReadAllText(path)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
        };

    /// <summary>
    /// <para>
    /// Creates a script task with given contents.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///    Script.Inline(
    ///        "echo \"Hello, world!\"",
    ///        "echo \"The time is $(Build.SourcesDirectory)\""
    ///    ) with { DisplayName = "A display name" }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - script: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'A display name'
    /// </code>
    /// </summary>
    /// <param name="scriptLines">Contents of the script</param>
    public ScriptTask Inline(params string[] scriptLines) => new(scriptLines);

    internal ScriptTaskBuilder()
    {
    }
}
