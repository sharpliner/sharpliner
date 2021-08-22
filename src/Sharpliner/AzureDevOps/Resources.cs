using Sharpliner.ConditionedDefinitions;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#resources">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record Resources
    {
        public ConditionedDefinitionList<ConditionedDefinition<PipelineResource>> Pipelines { get; init; } = new();

        /* TODO
        public ConditionedDefinitionList<ConditionedDefinition<BuildResource>> Builds { get; init; } = new();

        public ConditionedDefinitionList<ConditionedDefinition<RepositoryResource>> Repositories { get; init; } = new();
        */

        public ConditionedDefinitionList<ConditionedDefinition<ContainerResource>> Containers { get; init; } = new();

        public ConditionedDefinitionList<ConditionedDefinition<PackageResource>> Packages { get; init; } = new();
    }
}
