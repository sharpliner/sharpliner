using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

internal class MockPipeline : TestPipeline
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
            {
                new Stage("Build", "Build the project")
                {
                    Jobs =
                    {
                        new Job("Build_API", "Build API")
                        {
                            Variables =
                            {
                                If.Equal(variables["IsPr"], "true")
                                    .Variable("DotnetVersion", "6.0-preview-4"),

                                If.NotEqual(variables["IsPr"], "true")
                                    .Variable("DotnetVersion", "6.0"),
                            },

                            Steps =
                            {
                                DotNet.Pack("ProjectFile") with
                                {
                                    Inputs = new()
                                    {
                                        {
                                            If.Equal(parameters["IncludeSymbols"], "true"), new TaskInputs()
                                            {
                                                ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg"
                                            }
                                        },
                                        {
                                            Else, new TaskInputs()
                                            {
                                                ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build"
                                            }
                                        }
                                    },
                                },
                            }
                        }
                    }
                },

                If.Equal(variables["IsPr"], "true")
                    .Stage(new("Test", "Run E2E tests")
                    {
                        DependsOn = "Build",
                        Jobs =
                        {
                            // ...
                        }
                    })
            }
    };
}
