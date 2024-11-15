using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.SourceGenerator;

namespace Sharpliner.Tests.AzureDevOps;

internal class PlaygroundPipeline : TestPipeline
{
    public override Pipeline Pipeline => new()
    {
        Name = "$(Date:yyyMMdd).$(Rev:rr)",

        Trigger = new Trigger
        {
            Batch = false,
            Branches = new()
            {
                Include =
                {
                    "main",
                    "release/*",
                }
            }
        },

        Pr = new PrTrigger("main", "release/*"),

        Variables =
        {
            new Variable("Configuration", "Release"), // We can create the objects and then resue them for definition too
            Variable("Configuration", "Release"),     // Or we have this more YAML-like definition
            Group("PR keyvault variables"),
            If.IsPullRequest
                .Variable("TargetBranch", variables.System.PullRequest.SourceBranch)
                .Variable("IsPr", true),
            If.And(IsBranch("production"), NotEqual("Configuration", "Debug"))
                .Variable("PublishProfileFile", "Prod")
                .If.IsNotPullRequest
                    .Variable("AzureSubscription", "Int")
                    .Group("azure-int")
                .EndIf
                .If.IsPullRequest
                    .Variable("AzureSubscription", "Prod")
                    .Group("azure-prod"),
        },

        Stages =
        [
            Stage("First") with
            {
                Jobs =
                [
                    new JavaTemplate(new()
                    {
                        JavaVersion = "11",
                        MavenPomFile = "pom.xml",
                    })
                ]
            }
        ]
    };
}

[SharplinerTemplateParameters]
partial class JavaTemplate : JobTemplateDefinition<JavaTemplateParameters>
{
    public JavaTemplate(JavaTemplateParameters parameters) : base(parameters)
    {
    }

    public override string TargetFile => "java-template.yml";
    public override ConditionedList<JobBase> Definition =>
    [
        Job("Build") with
            {
                Steps =
                [
                    Task("UseJavaVersion@0") with
                    {
                        Inputs = new()
                        {
                            ["versionSpec"] = JavaVersionParameter,
                            ["jdkArchitecture"] = "x64",
                        }
                    },
                    Task("Maven@3") with
                    {
                        Inputs = new()
                        {
                            ["mavenPomFile"] = MavenPomFileParameter,
                            ["mavenOptions"] = "-Xmx3072m",
                            ["javaHomeOption"] = "JDKVersion",
                            ["jdkVersionOption"] = JavaVersionParameter,
                            ["jdkArchitectureOption"] = "x64",
                            ["publishJUnitResults"] = "false",
                            ["testResultsFiles"] = "**/surefire-reports/TEST-*.xml",
                            ["goals"] = "package",
                        }
                    }
                ]
            }
    ];
}

// TODO: This should be generated
partial class JavaTemplate
{
    private static readonly JavaTemplateParameters defaultParameters = new();

    protected static StringParameter JavaVersionParameter { get; } = new("JavaVersion", defaultValue: defaultParameters.JavaVersion);
    protected static StringParameter MavenPomFileParameter { get; } = new("MavenPomFile", defaultValue: defaultParameters.MavenPomFile);

    public override List<Parameter> Parameters => [JavaVersionParameter, MavenPomFileParameter];
}

class JavaTemplateParameters : TemplateParametersProviderBase<JavaTemplateParameters>
{
    public string? JavaVersion { get; set; }
    public string? MavenPomFile { get; set; }
}
