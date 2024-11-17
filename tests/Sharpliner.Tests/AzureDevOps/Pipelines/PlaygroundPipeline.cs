using System.Collections.Generic;
using System.Runtime.Serialization;
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
                            ["versionSpec"] = parameters.JavaVersion,
                            ["jdkArchitecture"] = "x64",
                        }
                    },
                    Task("Maven@3") with
                    {
                        Inputs = new()
                        {
                            ["mavenPomFile"] = parameters.MavenPomFile,
                            ["mavenOptions"] = parameters.MavenOptions,
                            ["javaHomeOption"] = "JDKVersion",
                            ["jdkVersionOption"] = parameters.JavaVersion,
                            ["jdkArchitectureOption"] = "x64",
                            ["publishJUnitResults"] = "false",
                            ["testResultsFiles"] = "**/surefire-reports/TEST-*.xml",
                            ["goals"] = "package",
                            ["spring.profiles.active"] = parameters.Spring.SpringProfile,
                            ["spring.options"] = parameters.Spring.SpringOptions,
                        }
                    }
                ]
            }
    ];
}

// TODO: This should be generated
/*
partial class JavaTemplate
{
    protected static new readonly JavaTemplateParametersReference parameters = new();

    public class JavaTemplateParametersReference : TemplateParameterReference
    {
        public ParameterReference JavaVersion => new("javaVersion");
        public ParameterReference MavenPomFile => new("mavenPomFile");
        public ParameterReference MavenOptions => new("maven-options");
        public SpringParametersParameterReference Spring => new("spring");
        public class SpringParametersParameterReference(string parameterName) : Sharpliner.AzureDevOps.ConditionedExpressions.ParameterReference(parameterName)
        {
            public ParameterReference SpringProfile => new("springProfile");
            public ParameterReference SpringOptions => new("spring-options");
        }
    }
}
*/

class JavaTemplateParameters : TemplateParametersProviderBase<JavaTemplateParameters>
{
    public string? JavaVersion { get; set; }
    public string? MavenPomFile { get; set; }

    [DataMember(Name = "maven-options")]
    public string? MavenOptions { get; set; } = "-Xmx3072m";

    public SpringParameters? Spring { get; set; } = new();
}

public record SpringParameters
{
    public string? SpringProfile { get; set; }

    [DataMember(Name = "spring-options")]
    public string? SpringOptions { get; set; } = "--server.port=8080";
}
