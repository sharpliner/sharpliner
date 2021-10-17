using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.CI
{
    internal class InstallDotNetTemplate : StepTemplateDefinition
    {
        public const string Path = Pipelines.TemplateLocation + "install-dotnet.yml";

        public override string TargetFile => Pipelines.Location + Path;

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override List<TemplateParameter> Parameters => new()
        {
            StringParameter("version"),
        };

        public override ConditionedList<Step> Definition => new()
        {
            // dotnet build fails with .NET 5 SDK and the new() statements
            DotNet
                .Install.Sdk(parameters["version"])
                .DisplayAs("Install .NET " + parameters["version"]),
        };
    }
}
