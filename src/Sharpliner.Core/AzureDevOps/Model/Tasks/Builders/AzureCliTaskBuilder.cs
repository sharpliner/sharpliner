using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating an Azure CLI task using the <c>AzureCli</c> keyword or the AzureCli task.
/// </summary>
public class AzureCliTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates an Azure CLI task where the contents come from an embedded resource.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="resourceFileName">Name of the resource file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlineAzureCliTask"/> with the contents of the resource file</returns>
    public InlineAzureCliTask FromResourceFile(string azureSubscription, AdoExpression<ScriptType> scriptType, string resourceFileName, AdoExpression<string>? displayName = null!)
        => new InlineAzureCliTask(azureSubscription, scriptType, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
        {
            DisplayName = displayName!,
        };

    /// <summary>
    /// Creates an Azure CLI task where the contents come from a file.
    /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="path">Path to the file</param>
    /// <param name="displayName">Display name of the build step</param>
    /// <returns>A new instance of <see cref="InlineAzureCliTask"/> with the contents of the file</returns>
    public InlineAzureCliTask FromFile(string azureSubscription, AdoExpression<ScriptType> scriptType, string path, AdoExpression<string>? displayName = null!)
        => new InlineAzureCliTask(azureSubscription, scriptType, System.IO.File.ReadAllText(path)) with
    {
        DisplayName = displayName!,
    };

    /// <summary>
    /// Creates an Azure CLI task referencing a file (contents are not inlined in the YAML).
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptPath">Path to the script</param>
    /// <param name="displayName">Name of the build step</param>
    /// <returns>A new instance of <see cref="AzureCliFileTask"/> with the file path</returns>
    public AzureCliFileTask File(string azureSubscription, AdoExpression<ScriptType> scriptType, string scriptPath, AdoExpression<string>? displayName = null) => new(azureSubscription, scriptType,scriptPath)
    {
        DisplayName = displayName!,
        ScriptPath = scriptPath,
    };

    /// <summary>
    /// Creates an Azure CLI task with given contents.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptLines">Contents of the script</param>
    /// <param name="displayName">Name of the build step</param>
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
