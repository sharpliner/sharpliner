using System.Collections.Generic;

namespace Sharpliner;

/// <summary>
/// Every collection of definitions that implements this interface will get serialized into YAML files.
/// We recommend to inherit from some of the more concrete definitions such as TemplateDefinitionCollection.
/// </summary>
public interface ISharplinerDefinitionCollection
{
    public IEnumerable<ISharplinerDefinition> Definitions { get; }
}
