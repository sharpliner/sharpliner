These NuGet.Tests are E2E testing following scenario:
1. We create a "library" project with some Sharpliner definition
2. We publish a .nupkg of this library project
3. We reference this .nupkg in another project

This is testing that all the referenced DLLs are loaded well even for this transitive scenario.
This is testing user scenarios where people want to publish definitions in the form of NuGet and re-use it in other projects.
