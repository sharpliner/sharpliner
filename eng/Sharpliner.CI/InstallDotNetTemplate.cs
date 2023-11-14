using System.Collections.Generic;
using Sharpliner.AzureDevOps;

namespace Sharpliner.CI;

class InstallDotNetTemplate : StepTemplateCollection
{
    public override List<TemplateDefinitionData<Step>> Templates =>
    [
        GetTemplate(false),
        GetTemplate(true),
    ];

    private static TemplateDefinitionData<Step> GetTemplate(bool isFullSdk)
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
            [step],
            [StringParameter("version")]);
    }
}
