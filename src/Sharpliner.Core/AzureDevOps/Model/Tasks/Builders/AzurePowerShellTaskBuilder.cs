using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating an Azure PowerShell task using the <c>AzurePowerShell</c> keyword.
/// The generated YAML uses the <c>AzurePowerShell@5</c> task as defined by the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-powershell-v5?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public class AzurePowerShellTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates an Azure PowerShell task where the contents come from an embedded resource.
    /// The script is emitted as <c>ScriptType: InlineScript</c>.
    /// <para>For example: assuming the embedded resource file <c>deploy.ps1</c> contains <c>Get-AzResourceGroup</c>:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzurePowerShell.FromResourceFile("myServiceConnection", "deploy.ps1", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzurePowerShell@5
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     ScriptType: InlineScript
    ///     Inline: |
    ///       Get-AzResourceGroup
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="resourceFileName">Name of the resource file.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="InlineAzurePowerShellTask"/> with the contents of the resource file</returns>
    public InlineAzurePowerShellTask FromResourceFile(string azureSubscription, string resourceFileName, AdoExpression<string>? displayName = null)
        => new(azureSubscription, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName))
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Creates an Azure PowerShell task where the contents come from a file.
    /// The contents are inlined in the YAML as opposed to the File method where the file name is just referenced.
    /// The script is emitted as <c>ScriptType: InlineScript</c>.
    /// <para>For example: assuming the file <c>scripts/deploy.ps1</c> contains <c>Get-AzResourceGroup</c>:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzurePowerShell.FromFile("myServiceConnection", "scripts/deploy.ps1", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzurePowerShell@5
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     ScriptType: InlineScript
    ///     Inline: |
    ///       Get-AzResourceGroup
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="path">Path to the file.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="InlineAzurePowerShellTask"/> with the contents of the file</returns>
    public InlineAzurePowerShellTask FromFile(string azureSubscription, string path, AdoExpression<string>? displayName = null)
        => new(azureSubscription, System.IO.File.ReadAllText(path))
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Creates an Azure PowerShell task referencing a script file (contents are not inlined in the YAML).
    /// The script is emitted as <c>ScriptType: FilePath</c>.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzurePowerShell.File("myServiceConnection", "scripts/deploy.ps1", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzurePowerShell@5
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     ScriptType: FilePath
    ///     ScriptPath: scripts/deploy.ps1
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="scriptPath">Path of the script to run.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="AzurePowerShellFileTask"/> with the file path</returns>
    public AzurePowerShellFileTask File(string azureSubscription, string scriptPath, AdoExpression<string>? displayName = null)
        => new(azureSubscription, scriptPath)
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Creates an Azure PowerShell task with given contents.
    /// The script is emitted as <c>ScriptType: InlineScript</c>.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzurePowerShell.Inline(
    ///         "myServiceConnection",
    ///         "Deploy resources",
    ///         "Get-AzResourceGroup",
    ///         "New-AzResourceGroup -Name myRG -Location eastus"
    ///     )
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzurePowerShell@5
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     ScriptType: InlineScript
    ///     Inline: |
    ///       Get-AzResourceGroup
    ///       New-AzResourceGroup -Name myRG -Location eastus
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <param name="scriptLines">Contents of the script.</param>
    /// <returns>A new instance of <see cref="InlineAzurePowerShellTask"/> with the script lines</returns>
    public InlineAzurePowerShellTask Inline(string azureSubscription, AdoExpression<string>? displayName = null, params string[] scriptLines)
        => new(azureSubscription, string.Join("\n", scriptLines))
        {
            DisplayName = displayName!,
        };

    internal AzurePowerShellTaskBuilder()
    {
    }
}
