using E2E.Tests.SharplinerLibrary;
using Sharpliner;

namespace E2E.Tests.ProjectUsingTheLibrary;

// These NuGet.Tests are E2E testing following scenario:
// 1. We create a "library" project with an arbitrary Sharpliner definition
// 2. We reference this library in another project directly
// 3. We define a non-abstract definition class without a parameterless constructor

// This is testing that all the referenced DLLs are loaded well even for this transitive scenario.
// This is testing user scenarios where people want to publish definitions in the form of NuGet and re-use it in other projects.

// This class is the one that uses the library directly
public class BasicDefinition(string definitionName) : BasePipelineFromLibrary
{
    public override string TargetFile => Path.Combine("artifacts", "E2E.Tests", $"{definitionName}.yml");

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
}

public class ProjectTestDefinition() : BasicDefinition("ProjectTestDefinition")
{
}