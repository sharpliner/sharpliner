using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Creates <c>AzureCLI@3</c> tasks using the <c>AzureCliV3</c> keyword.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v3?view=azure-pipelines">official Azure DevOps task reference</see>.
/// </summary>
public class AzureCliV3TaskBuilder : TaskBuilderBase
{
    /// <summary>Creates an AzureCLI@3 task whose inline script is read from an embedded resource.</summary>
    public InlineAzureCliV3Task FromResourceFile(AzureCliV3ConnectionType connectionType, string serviceConnection, AdoExpression<ScriptType> scriptType, string resourceFileName, AdoExpression<string>? displayName = null!)
        => new(connectionType, serviceConnection, scriptType, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) { DisplayName = displayName! };

    /// <summary>Creates an AzureCLI@3 task whose inline script is read from a file.</summary>
    public InlineAzureCliV3Task FromFile(AzureCliV3ConnectionType connectionType, string serviceConnection, AdoExpression<ScriptType> scriptType, string path, AdoExpression<string>? displayName = null!)
        => new(connectionType, serviceConnection, scriptType, System.IO.File.ReadAllText(path)) { DisplayName = displayName! };

    /// <summary>Creates an AzureCLI@3 task that references a script file.</summary>
    public AzureCliV3FileTask File(AzureCliV3ConnectionType connectionType, string serviceConnection, AdoExpression<ScriptType> scriptType, string scriptPath, AdoExpression<string>? displayName = null)
        => new(connectionType, serviceConnection, scriptType, scriptPath) { DisplayName = displayName! };

    /// <summary>Creates an AzureCLI@3 task with an inline script.</summary>
    public InlineAzureCliV3Task Inline(AzureCliV3ConnectionType connectionType, string serviceConnection, AdoExpression<ScriptType> scriptType, AdoExpression<string>? displayName = null, params string[] scriptLines)
        => new(connectionType, serviceConnection, scriptType, string.Join("\n", scriptLines)) { DisplayName = displayName! };

    internal AzureCliV3TaskBuilder()
    {
    }
}
