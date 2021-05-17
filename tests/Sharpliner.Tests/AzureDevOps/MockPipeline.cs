using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps
{
    internal class MockPipeline : AzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override AzureDevOpsPipeline Pipeline => new()
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

                If.Equal(variables["Build.Reason"], "PullRequest")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)")
                    .Variable("IsPr", true),

                If.And(Equal(variables["Build.SourceBranch"], "refs/heads/production"), NotEqual("Configuration", "Debug"))
                    .Variable("PublishProfileFile", "Prod")
                    .If.NotEqual(variables["Build.Reason"], "PullRequest")
                        .Variable("AzureSubscription", "Int")
                        .Group("azure-int")
                    .EndIf()
                    .If.Equal(variables["Build.Reason"], "PullRequest")
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
                                new InlineBashTask("Restore .NET",
                                    "curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh",
                                    "chmod +x dotnet-install.sh",
                                    "./dotnet-install.sh --channel $(DotnetVersion) --install-dir ./.dotnet"),

                                new InlineBashTask("Build .sln",
                                    "./.dotnet/dotnet build src/MySolution.sln -c $(Configuration)"),

                                new InlineBashTask("Run unit tests",
                                    "./.dotnet/dotnet test src/MySolution.sln")
                                {
                                    ContinueOnError = true,
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },

                                new InlineBashTask("Upload tests results",
                                    "./upload.sh ./**/TestResult.xml")
                                {
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },
                            }
                        }
                    }
                },

                If_<Stage>().Equal(variables["IsPr"], "true")
                    .Stage(new("Test", "Run E2E tests")
                    {
                        DependsOn = { "Build" },
                        Jobs =
                        {
                            // ...
                        }
                    })
            }
        };
    }
}
