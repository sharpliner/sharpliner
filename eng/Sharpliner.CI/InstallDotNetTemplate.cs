using Sharpliner.AzureDevOps;

namespace Sharpliner.CI
{
    internal class InstallDotNetTemplate : StepTemplateDefinition
    {
        public const string Path = Pipelines.TemplateLocation + "install-dotnet.yml";

        public override string TargetFile => Pipelines.Location + Path;

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override TemplateParameters Parameters => NoParameters;

        public override ConditionedDefinitionList<Step> Definition => new()
        {
            // dotnet build fails with .NET 5 SDK and the new() statements
            DotNet
                .Install.Sdk("6.0.100-preview.3.21202.5")
                .DisplayAs("Install .NET 6 preview 3")
        };
    }
}
