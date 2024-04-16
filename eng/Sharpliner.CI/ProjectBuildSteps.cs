using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.CI;

class ProjectBuildSteps(string project) : StepLibrary
{
    public const string PackagePath = "artifacts/packages";

    public override List<Conditioned<Step>> Steps =>
    [
        StepTemplate(Pipelines.TemplateLocation + "install-dotnet-sdk.yml", new()
        {
            { "version", "8.0.204" }
        }),

        Powershell
            .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
            .DisplayAs($"Create {PackagePath}"),

        DotNet
            .Build(project, includeNuGetOrg: true)
            .DisplayAs("Build"),
    ];
}
