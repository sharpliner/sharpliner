using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a bash task using the <c>bash</c> keyword or the Bash task.
/// </summary>
public class BashTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// <para>
    /// Creates a bash task where the contents come from an embedded resource.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// assuming the embedded resource file <c>my-script.sh</c> contains:
    /// <code>
    /// echo "Hello, world!"
    /// echo "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// and with the code:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///    Bash.FromResourceFile("my-script.sh", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - bash: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'Run a script'
    /// </code>
    /// </summary>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlineBashTask"/> with the contents of the resource file</returns>
    public InlineBashTask FromResourceFile(string resourceFileName, string? displayName = null!)
        => new InlineBashTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
        };

    /// <summary>
    /// <para>
    /// Creates a bash task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// assuming the file <c>AzureDevOps/Resources/script.sh</c> contains:
    /// <code>
    /// echo "Hello, world!"
    /// echo "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// and with the code:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Bash.FromFile("AzureDevOps/Resources/my-script.sh", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - bash: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'Run a script'
    /// </code>
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlineBashTask"/> with the contents of the file</returns>
    public InlineBashTask FromFile(string path, string? displayName = null!) => new InlineBashTask(System.IO.File.ReadAllText(path)) with
    {
        DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
    };

    /// <summary>
    /// <para>
    /// Creates a bash task referencing a bash file (contents are not inlined in the YAML).
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Bash.File("AzureDevOps/Resources/my-script.sh", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: Bash@3
    ///   displayName: 'Run a script'
    ///   inputs:
    ///     targetType: 'filePath'
    ///     filePath: 'AzureDevOps/Resources/my-script.sh'
    /// </code>
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="displayName">Name of the build step</param>
    /// <returns>A new instance of <see cref="BashFileTask"/> with the file path</returns>
    public BashFileTask File(string filePath, string? displayName = null) => new(filePath)
    {
        DisplayName = displayName is null ? null! : new Conditioned<string>(displayName),
    };

    /// <summary>
    /// <para>
    /// Creates a bash task with given contents.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Bash.Inline(
    ///         "echo \"Hello, world!\"",
    ///         "echo \"The time is $(Build.SourcesDirectory)\""
    ///     ) with { DisplayName = "A display name" }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - bash: |
    ///     echo "Hello, world!"
    ///     echo "The time is $(Build.SourcesDirectory)"
    ///   displayName: 'A display name'
    /// </code>
    /// </summary>
    /// <param name="scriptLines">Contents of the script</param>
    /// <returns>A new instance of <see cref="InlineBashTask"/> with the script lines</returns>
    public InlineBashTask Inline(params string[] scriptLines) => new(scriptLines);

    internal BashTaskBuilder()
    {
    }
}
