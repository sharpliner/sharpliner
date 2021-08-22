using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps
{
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

                If.Equal(variables["Build.Reason"], "PullRequest")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)")
                    .Variable("IsPr", true),

                If.And(Equal(variables["Build.SourceBranch"], "refs/heads/production"), NotEqual("Configuration", "Debug"))
                    .Variable("PublishProfileFile", "Prod")
                    .If.NotEqual(variables["Build.Reason"], "PullRequest")
                        .Variable("AzureSubscription", "Int")
                        .Group("azure-int")
                    .EndIf
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
                                Bash.Inline(
                                    "curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh",
                                    "chmod +x dotnet-install.sh",
                                    "./dotnet-install.sh --channel $(DotnetVersion) --install-dir ./.dotnet") with
                                {
                                    DisplayName = "Restore .NET"
                                },

                                Bash.Inline("./.dotnet/dotnet build src/MySolution.sln -c $(Configuration)") with
                                {
                                    DisplayName = "Build .sln"
                                },

                                Bash.Inline("./.dotnet/dotnet test src/MySolution.sln") with
                                {
                                    DisplayName = "Run unit tests",
                                    ContinueOnError = true,
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },

                                Bash.Inline("./upload.sh ./**/TestResult.xml") with
                                {
                                    DisplayName = "Upload tests results",
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },
                            }
                        }
                    }
                },

                If.Equal(variables["IsPr"], "true")
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
