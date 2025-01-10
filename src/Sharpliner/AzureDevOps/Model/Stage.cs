using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Represents a stage in an Azure DevOps pipeline. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/stages-stage">stages.stage definition</see> for more details.
/// </summary>
public record Stage : IDependsOn
{
    /// <summary>
    /// Name of the stage (A-Z, a-z, 0-9, and underscore).
    /// </summary>
    [YamlMember(Alias = "stage", Order = 1)]
    public string Name { get; }

    /// <summary>
    /// Friendly name to display in the UI
    /// </summary>
    [YamlMember(Order = 2)]
    [DisallowNull]
    public Conditioned<string>? DisplayName { get; init; }

    /// <summary>
    /// The lock behavior of the stage
    /// </summary>
    [YamlMember(Order = 3)]
    public Conditioned<LockBehavior>? LockBehavior { get; init; }

    /// <summary>
    /// <para>
    /// List of names of other stages this stage depends on
    /// </para>
    /// <para>
    /// To specify this stage should run in parallel set this to an empty list or the utility <see cref="AzureDevOpsDefinition.NoDependsOn"/>
    /// </para>
    /// </summary>
    [YamlMember(Order = 100, DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public DependsOn? DependsOn { get; init; }

    /// <summary>
    /// Specifies variables at the stage level
    /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
    /// </summary>
    [YamlMember(Order = 200)]
    public ConditionedList<VariableBase> Variables { get; init; } = [];

    /// <summary>
    /// A job is a collection of steps run by an agent or on a server.
    /// Jobs can run conditionally and might depend on earlier jobs.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/phases?tabs=yaml&amp;view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 300)]
    public ConditionedList<JobBase> Jobs { get; init; } = [];

    /// <summary>
    /// Gets the condition expression to determine whether to run this stage.
    /// </summary>
    [YamlMember(Order = 400)]
    [DisallowNull]
    public InlineCondition? Condition { get; init; }

    /// <summary>
    /// Instantiates a new instance of <see cref="Stage"/> with the specified name and optional display name.
    /// </summary>
    /// <param name="name">The name of the stage.</param>
    /// <param name="displayName">The optional friendly name to display in the UI.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public Stage(string name, string? displayName = null)
    {
        Name = name ?? throw new System.ArgumentNullException(nameof(name));

        if (displayName != null)
        {
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Sets the displayName property.
    /// </summary>
    public Stage DisplayAs(string displayName) => this with { DisplayName = displayName };
}
