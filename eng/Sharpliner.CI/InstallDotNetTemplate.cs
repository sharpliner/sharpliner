using System.Collections.Generic;
using Sharpliner.AzureDevOps;

namespace Sharpliner.CI
{
    internal class InstallDotNetTemplate : StepTemplateCollection
    {
        public override IEnumerable<TemplateDefinitionData<Step>> Templates => new[]
        {
            GetTemplate(false),
            GetTemplate(true),
        };

        private TemplateDefinitionData<Step> GetTemplate(bool isFullSdk)
        {
            var targetFile = Pipelines.Location + Pipelines.TemplateLocation + $"install-dotnet-{(isFullSdk ? "sdk" : "runtime")}.yml";
            Step step;

            if (isFullSdk)
            {
                step = DotNet.Install.Sdk(parameters["version"])
                    .DisplayAs("Install .NET SDK " + parameters["version"]);
            }
            else
            {
                step = DotNet.Install.Runtime(parameters["version"])
                    .DisplayAs("Install .NET runtime " + parameters["version"]);
            }

            return new(
                targetFile,
                new() { step },
                new() { StringParameter("version") });
        }
    }
}
