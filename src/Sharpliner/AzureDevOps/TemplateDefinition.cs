using System.Diagnostics.CodeAnalysis;
using Sharpliner.Definition;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract class TemplateDefinition : PipelineDefinitionBase
    {
        [YamlMember(Order = 1)]
        [DisallowNull]
        public abstract TemplateParameters Parameters { get; }
    }
}
