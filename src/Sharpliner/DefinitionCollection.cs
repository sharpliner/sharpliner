using System.Collections.Generic;

namespace Sharpliner;

/// <summary>
/// Use this data class to create pipeline definitions dynamically inside of PipelineDefinitionCollection.
/// </summary>
/// <typeparam name="TDefinition">Type of the definition</typeparam>
public abstract class DefinitionCollection<TDefinition> where TDefinition : ISharplinerDefinition
{
    /// <summary>
    /// If set, override's the target directory set for the parent collection
    /// </summary>
    public string? TargetDirectory { get; set; }

    /// <summary>
    /// The definition itself
    /// </summary>
    public abstract IEnumerable<TDefinition> Definitions { get; }

    // No inheritance outside of this project to not confuse people.
    internal DefinitionCollection()
    {
    }
}
