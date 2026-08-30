using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating an Azure CLI task using the <c>AzureCli</c> keyword.
/// The generated YAML uses the <c>AzureCLI@2</c> task as defined by the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/9dabcbcbcbc3b5a1a94fd32acaa2766fdf934bd6/Tasks/AzureCLIV2/task.json">official AzureCLIV2 task specification audited on 2026-08-30</see>.
/// </summary>
public class AzureCliTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates an Azure CLI task where the contents come from an embedded resource.
    /// The script is emitted as <c>scriptLocation: inlineScript</c>.
    /// <para>For example: assuming the embedded resource file <c>deploy.sh</c> contains <c>az group create --name myRG --location eastus</c>:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureCli.FromResourceFile("myServiceConnection", ScriptType.Bash, "deploy.sh", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureCLI@2
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     scriptType: bash
    ///     scriptLocation: inlineScript
    ///     inlineScript: |
    ///       az group create --name myRG --location eastus
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script. Use <see cref="ScriptType.Bash"/> or <see cref="ScriptType.Pscore"/> on Linux agents; use <see cref="ScriptType.Batch"/>, <see cref="ScriptType.Ps"/>, or <see cref="ScriptType.Pscore"/> on Windows agents.</param>
    /// <param name="resourceFileName">Name of the resource file.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="InlineAzureCliTask"/> with the contents of the resource file</returns>
    public InlineAzureCliTask FromResourceFile(string azureSubscription, AdoExpression<ScriptType> scriptType, string resourceFileName, AdoExpression<string>? displayName = null!)
        => new InlineAzureCliTask(azureSubscription, scriptType, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Creates an Azure CLI task where the contents come from a file.
    /// The contents are inlined in the YAML as opposed to the File method where the file name is just referenced.
    /// The script is emitted as <c>scriptLocation: inlineScript</c>.
    /// <para>For example: assuming the file <c>scripts/deploy.sh</c> contains <c>az group create --name myRG --location eastus</c>:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureCli.FromFile("myServiceConnection", ScriptType.Bash, "scripts/deploy.sh", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureCLI@2
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     scriptType: bash
    ///     scriptLocation: inlineScript
    ///     inlineScript: |
    ///       az group create --name myRG --location eastus
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script. Use <see cref="ScriptType.Bash"/> or <see cref="ScriptType.Pscore"/> on Linux agents; use <see cref="ScriptType.Batch"/>, <see cref="ScriptType.Ps"/>, or <see cref="ScriptType.Pscore"/> on Windows agents.</param>
    /// <param name="path">Path to the file.</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="InlineAzureCliTask"/> with the contents of the file</returns>
    public InlineAzureCliTask FromFile(string azureSubscription, AdoExpression<ScriptType> scriptType, string path, AdoExpression<string>? displayName = null!)
        => new InlineAzureCliTask(azureSubscription, scriptType, System.IO.File.ReadAllText(path)) with
    {
        DisplayName = displayName!,
    };

    /// <summary>
    /// Creates an Azure CLI task referencing a file (contents are not inlined in the YAML).
    /// The script is emitted as <c>scriptLocation: scriptPath</c>.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureCli.File("myServiceConnection", ScriptType.Bash, "scripts/deploy.sh", "Deploy resources")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureCLI@2
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     scriptType: bash
    ///     scriptLocation: scriptPath
    ///     scriptPath: scripts/deploy.sh
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script. Use <see cref="ScriptType.Bash"/> or <see cref="ScriptType.Pscore"/> on Linux agents; use <see cref="ScriptType.Batch"/>, <see cref="ScriptType.Ps"/>, or <see cref="ScriptType.Pscore"/> on Windows agents.</param>
    /// <param name="scriptPath">Path to the script. Use <c>.ps1</c>, <c>.bat</c>, or <c>.cmd</c> on Windows agents; use <c>.ps1</c> or <c>.sh</c> on Linux agents.</param>
    /// <param name="displayName">Name of the build step.</param>
    /// <returns>A new instance of <see cref="AzureCliFileTask"/> with the file path</returns>
    public AzureCliFileTask File(string azureSubscription, AdoExpression<ScriptType> scriptType, string scriptPath, AdoExpression<string>? displayName = null) => new(azureSubscription, scriptType, scriptPath)
    {
        DisplayName = displayName!,
        ScriptPath = scriptPath,
    };

    /// <summary>
    /// Creates an Azure CLI task with given contents.
    /// The script is emitted as <c>scriptLocation: inlineScript</c>.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     AzureCli.Inline(
    ///         "myServiceConnection",
    ///         ScriptType.Bash,
    ///         "Deploy resources",
    ///         "az group create --name myRG --location eastus",
    ///         "az deployment group create --resource-group myRG --template-file azuredeploy.json"
    ///     )
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: AzureCLI@2
    ///   displayName: Deploy resources
    ///   inputs:
    ///     azureSubscription: myServiceConnection
    ///     scriptType: bash
    ///     scriptLocation: inlineScript
    ///     inlineScript: |
    ///       az group create --name myRG --location eastus
    ///       az deployment group create --resource-group myRG --template-file azuredeploy.json
    /// </code>
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script. Use <see cref="ScriptType.Bash"/> or <see cref="ScriptType.Pscore"/> on Linux agents; use <see cref="ScriptType.Batch"/>, <see cref="ScriptType.Ps"/>, or <see cref="ScriptType.Pscore"/> on Windows agents.</param>
    /// <param name="scriptLines">Contents of the script.</param>
    /// <param name="displayName">Name of the build step.</param>
    /// <returns>A new instance of <see cref="InlineAzureCliTask"/> with the script lines</returns>
    public InlineAzureCliTask Inline(string azureSubscription, AdoExpression<ScriptType> scriptType, AdoExpression<string>? displayName = null, params string[] scriptLines)
        => new(azureSubscription, scriptType, string.Join("\n", scriptLines))
        {
            DisplayName = displayName!,
        };

    internal AzureCliTaskBuilder()
    {
    }
}
