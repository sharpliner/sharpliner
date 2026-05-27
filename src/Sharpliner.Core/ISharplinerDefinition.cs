using Sharpliner.Common;
using System.Collections.Generic;

namespace Sharpliner;

/// <summary>
/// Every class that implements this interface will be serialized into a YAML file.
/// We recommend to inherit from some of the more concrete definitions such as PipelineDefinition.
/// </summary>
public interface ISharplinerDefinition
{
    /// <summary>
    /// Path to the YAML file/folder where this definition/collection will be exported to
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    string TargetFile { get; }

    /// <summary>
    /// Override this to define where the resulting YAML should be stored (together with TargetFile)
    /// Default is RelativeToGit
    /// </summary>
    TargetPathType TargetPathType { get; }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// Leave empty array to omit file header
    /// </summary>
    string[]? Header { get; }

    /// <summary>
    /// Returns the list of validations that should be run on the definition (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    IReadOnlyCollection<IDefinitionValidation> Validations { get; }

    /// <summary>
    /// Serializes the definition into a YAML string
    /// </summary>
    string Serialize();
}
