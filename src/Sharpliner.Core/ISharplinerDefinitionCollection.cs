using System.Collections.Generic;

namespace Sharpliner;

/// <summary>
/// Every collection of definitions that implements this interface will get serialized into YAML files.
/// We recommend to inherit from some of the more concrete definitions such as <see cref="AzureDevOps.TemplateDefinitionCollection{T}"/>.
/// </summary>
public interface ISharplinerDefinitionCollection
{
    /// <summary>
    /// Returns a sequence of definitions where each definition will be serialized to a YAML file, e.g. pipelines, stages, jobs, steps, variables.
    /// </summary>
    public IEnumerable<ISharplinerDefinition> Definitions { get; }
}
