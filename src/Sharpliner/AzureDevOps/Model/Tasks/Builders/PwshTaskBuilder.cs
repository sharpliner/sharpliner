using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a bash task using the <c>pwsh</c> keyword or the Powershell task.
/// </summary>
public class PwshTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// <para>
    /// Creates a Powershell task where the contents come from an embedded resource.
    /// </para>
    /// For example:
    /// <para>
    /// assuming the embedded resource file <c>my-script.ps1</c> contains:
    /// </para>
    /// <code>
    /// Write-Host "Hello, world!"
    /// Write-Host "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Pwsh.FromResourceFile("my-script.ps1")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - pwsh: |+
    ///     Write-Host "Hello, world!"
    ///     Write-Host "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// </summary>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlinePwshTask"/> with the contents of the resource file</returns>
    public InlinePwshTask FromResourceFile(string resourceFileName, AdoExpression<string>? displayName = null)
        => new InlinePwshTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName!
        };

    /// <summary>
    /// <para>
    /// Creates a Powershell task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </para>
    /// For example:
    /// <para>
    /// assuming the file <c>AzureDevOps/Resources/my-script.ps1</c> contains:
    /// </para>
    /// <code>
    /// Write-Host "Hello, world!"
    /// Write-Host "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Pwsh.FromFile("AzureDevOps/Resources/my-script.ps1")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - pwsh: |+
    ///     Write-Host "Hello, world!"
    ///     Write-Host "The time is $(Build.SourcesDirectory)"
    /// </code>
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlinePwshTask"/> with the contents of the file</returns>
    public InlinePwshTask FromFile(string path, AdoExpression<string>? displayName = null)
        => new InlinePwshTask(System.IO.File.ReadAllText(path)) with
        {
            DisplayName = displayName!
        };

    /// <summary>
    /// <para>
    /// Creates a Powershell task referencing a Powershell file (contents are not inlined in the YAML).
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Pwsh.File("AzureDevOps/Resources/my-script.ps1", "Run a script")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: PowerShell@2
    ///   displayName: 'Run a script'
    ///   inputs:
    ///     targetType: 'filePath'
    ///     filePath: 'AzureDevOps/Resources/my-script.ps1'
    ///     pwsh: true
    /// </code>
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="PowershellFileTask"/> with the contents of the file</returns>
    public PowershellFileTask File(AdoExpression<string> filePath, AdoExpression<string>? displayName = null)
        => new PowershellFileTask(filePath, isPwsh: true) with
        {
            DisplayName = displayName!
        };

    /// <summary>
    /// <para>
    /// Creates a Powershell task with given contents.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Pwsh.Inline(
    ///         "$Files = Get-ChildItem *.sln",
    ///         "Remove-Item $Files"
    ///     ) with { DisplayName = "A display name" }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - pwsh: |
    ///     $Files = Get-ChildItem *.sln
    ///     Remove-Item $Files
    ///   displayName: 'A display name'
    /// </code>
    /// </summary>
    /// <param name="scriptLines">Contents of the script</param>
    /// <returns>A new instance of <see cref="InlinePwshTask"/> with the contents of the script</returns>
    public InlinePwshTask Inline(params string[] scriptLines) => new(scriptLines);
}
