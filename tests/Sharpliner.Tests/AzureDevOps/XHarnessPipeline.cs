using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps
{
    /// <summary>
    /// More details can be found in <see href="https://github.com/dotnet/xharness/blob/main/azure-pipelines.yml">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    internal class XHarnessPipeline : PipelineDefinition
    {
        public override string TargetFile => "xharness.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                VariableTemplate("eng/common-variables.yml"),
                Variable("Build.Repository.Clean", true),
            },

            Trigger = new Trigger("main", "xcode/*")
            {
                Batch = true
            },

            Pr = new PrTrigger("main", "xcode/*"),

            Stages =
            {
                new Stage("Build_Windows_NT", "Build Windows")
                {
                    Jobs =
                    {
                        new Template<Job>("/eng/common/templates/jobs/jobs.yml")
                        {
                            Parameters =
                            {
                                { "enableTelemetry", true },
                                { "enablePublishBuildArtifacts", true },
                                { "enableMicrobuild", true },
                                { "enablePublishUsingPipelines", true },
                                { "enablePublishBuildAssets", true },
                                { "helixRepo", "dotnet/xharness" },
                                { "jobs", new[]
                                    {
                                        new Job("Windows_NT")
                                        {
                                            Pool =
                                                If.Equal(variables["_RunAsInternal"], "True")
                                                    .Pool(new HostedPool("NetCore1ESPool-Internal")
                                                    {
                                                        Demands = { "ImageOverride -equals Build.Server.Amd64.VS2019" }
                                                    })
                                                .EndIf
                                                .If.Equal(variables["_RunAsPublic"], "True")
                                                    .Pool(new HostedPool(vmImage: "windows-2019")),

                                            Strategy =
                                                If.Equal(variables["_RunAsPublic"], "True")
                                                    .Strategy(new MatrixStrategy
                                                    {
                                                        Matrix = new()
                                                        {
                                                            { "Release", new[] { ("_BuildConfig", "Release") } },
                                                            { "Debug", new[] { ("_BuildConfig", "Debug") } },
                                                        }
                                                    })
                                                .Else
                                                    .Strategy(new MatrixStrategy
                                                    {
                                                        Matrix = new()
                                                        {
                                                            { "Release", new[] { ("_BuildConfig", "Release") } },
                                                        }
                                                    }),

                                            Steps =
                                            {
                                                If.Equal(variables["_RunAsPublic"], "False")
                                                    .Step(Script.Inline(
                                                            "eng\\common\\CIBuild.cmd" +
                                                            " -configuration $(_BuildConfig)" +
                                                            " -prepareMachine" +
                                                            " $(_InternalBuildArgs)" +
                                                            " /p:Test=false")
                                                        .DisplayAs("Build")
                                                        .WhenSucceeded()),

                                                If.Equal(variables["_RunAsPublic"], "True")
                                                    .Step(Script.Inline(
                                                            "eng\\common\\CIBuild.cmd" +
                                                            " -configuration $(_BuildConfig)" +
                                                            " -prepareMachine" +
                                                            " $(_InternalBuildArgs)")
                                                        .DisplayAs("Build and run tests")
                                                        .WhenSucceeded())

                                                    .Step(new AzureDevOpsTask("PublishTestResults@2")
                                                    {
                                                        DisplayName = "Publish Unit Test Results",
                                                        Inputs =
                                                        {
                                                            { "testResultsFormat", "xUnit" },
                                                            { "testResultsFiles", "$(Build.SourcesDirectory)/artifacts/TestResults/**/*.xml" },
                                                            { "mergeTestResults", true },
                                                            { "searchFolder", "$(system.defaultworkingdirectory)" },
                                                            { "testRunTitle", "XHarness unit tests - $(Agent.JobName)" },
                                                        }
                                                    }.WhenSucceededOrFailed())

                                                    .Step(new AzureDevOpsTask("ComponentGovernanceComponentDetection@0")
                                                    {
                                                        DisplayName = "Component Governance scan",
                                                        Inputs =
                                                        {
                                                            { "ignoreDirectories", "$(Build.SourcesDirectory)/.packages,$(Build.SourcesDirectory)/artifacts/obj/Microsoft.DotNet.XHarness.CLI/$(_BuildConfig)/net6.0/android-tools-unzipped" },
                                                        }
                                                    }),
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },

                If.Equal(variables["_RunAsPublic"], "True")
                    .Stage(new Stage("Build_OSX", "Build OSX")
                    {
                        Jobs = {
                            new Template<Job>("/eng/common/templates/jobs/jobs.yml")
                            {
                                Parameters =
                                {
                                    { "enableTelemetry", true },
                                    { "enablePublishBuildArtifacts", true },
                                    { "enableMicrobuild", true },
                                    { "enablePublishUsingPipelines", true },
                                    { "enablePublishBuildAssets", true },
                                    { "helixRepo", "dotnet/xharness" },
                                    { "jobs", new[]
                                        {
                                            new Job("OSX")
                                            {
                                                DisplayName = "Build OSX",
                                                Pool = new Pool("Hosted macOS"),
                                                Strategy =
                                                    If.Equal(variables["_RunAsPublic"], "True")
                                                        .Strategy(new MatrixStrategy
                                                        {
                                                            Matrix = new()
                                                            {
                                                                { "Release", new[] { ("_BuildConfig", "Release") } },
                                                                { "Debug", new[] { ("_BuildConfig", "Debug") } },
                                                            }
                                                        })
                                                    .Else
                                                        .Strategy(new MatrixStrategy
                                                        {
                                                            Matrix = new()
                                                            {
                                                                { "Release", new[] { ("_BuildConfig", "Release") } },
                                                            }
                                                        }),

                                                Steps =
                                                {
                                                    If.Equal(variables["_RunAsPublic"], "False")
                                                        .Step(Script.Inline(
                                                                "eng/common/cibuild.sh" +
                                                                " --configuration $(_BuildConfig)" +
                                                                " --prepareMachine" +
                                                                " $(_InternalBuildArgs)" +
                                                                " /p:Test=false")
                                                            .DisplayAs("Build")
                                                            .WhenSucceeded()),

                                                    If.Equal(variables["_RunAsPublic"], "True")
                                                        .Step(Script.Inline(
                                                                "eng/common/cibuild.sh" +
                                                                " --configuration $(_BuildConfig)" +
                                                                " --prepareMachine" +
                                                                " $(_InternalBuildArgs)")
                                                            .DisplayAs("Build and run tests")
                                                            .WhenSucceeded())

                                                        .Step(new PublishTask(
                                                                "$(Build.SourcesDirectory)/artifacts/packages/$(_BuildConfig)/Shipping/Microsoft.DotNet.XHarness.CLI.1.0.0-ci.nupkg",
                                                                "Microsoft.DotNet.XHarness.CLI.$(_BuildConfig)")
                                                            .DisplayAs("Publish XHarness CLI for Helix Testing")
                                                            .When("and(succeeded(), eq(variables['_BuildConfig'], 'Debug'))"))

                                                        .Step(new AzureDevOpsTask("PublishTestResults@2")
                                                        {
                                                            DisplayName = "Publish Unit Test Results",
                                                            Inputs =
                                                            {
                                                                { "testResultsFormat", "xUnit" },
                                                                { "testResultsFiles", "$(Build.SourcesDirectory)/artifacts/TestResults/**/*.xml" },
                                                                { "mergeTestResults", true },
                                                                { "searchFolder", "$(system.defaultworkingdirectory)" },
                                                                { "testRunTitle", "XHarness unit tests - $(Agent.JobName)" },
                                                            }
                                                        }.WhenSucceededOrFailed())
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    })

                // E2E tests
                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Android_Simulators" },
                        { "displayName", "Android - Simulators" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Android/Simulator.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Android_Devices" },
                        { "displayName", "Android - Devices" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Android/Device.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Android_Manual_Commands" },
                        { "displayName", "Android - Manual Commands" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Android/Commands.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Apple_Simulators" },
                        { "displayName", "Apple - Simulators" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/Simulator.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_iOS_Devices" },
                        { "displayName", "Apple - iOS devices" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/Device.iOS.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_tvOS_Devices" },
                        { "displayName", "Apple - tvOS devices" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/Device.tvOS.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Apple_Simulator_Commands" },
                        { "displayName", "Apple - Simulator Commands" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/Simulator.Commands.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Apple_Device_Commands" },
                        { "displayName", "Apple - Device Commands" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/Device.Commands.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_Apple_Simulator_Mgmt" },
                        { "displayName", "Apple - Simulator management" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/Apple/SimulatorInstaller.Tests.proj" },
                    })

                    .Template("eng/e2e-test.yml", new TemplateParameters
                    {
                        { "name", "E2E_WASM" },
                        { "displayName", "WASM" },
                        { "testProject", "$(Build.SourcesDirectory)/tests/integration-tests/WASM/WASM.Helix.SDK.Tests.proj" },
                    }),

                // NuGet publishing
                If.Equal(variables["_RunAsInternal"], "True")
                    .Template<Stage>("eng/common/templates/post-build/post-build.yml", new TemplateParameters()
                    {
                        { "publishingInfraVersion", 3 },
                        { "enableSymbolValidation", true },
                        { "enableSourceLinkValidation", true },
                        { "validateDependsOn", new[] { "Build_Windows_NT" } },
                        { "publishDependsOn", new[] { "Validate" } },

                        // This is to enable SDL runs part of Post-Build Validation Stage
                        { "SDLValidationParameters", new TemplateParameters
                            {
                                { "enable", false },
                                { "continueOnError", false },
                                { "params", " -SourceToolsList @(\"policheck\",\"credscan\") " +
                                            "-TsaInstanceURL $(_TsaInstanceURL) " +
                                            "-TsaProjectName $(_TsaProjectName) " +
                                            "-TsaNotificationEmail $(_TsaNotificationEmail) " +
                                            "-TsaCodebaseAdmin $(_TsaCodebaseAdmin) " +
                                            "-TsaBugAreaPath $(_TsaBugAreaPath) " +
                                            "-TsaIterationPath $(_TsaIterationPath) " +
                                            "-TsaRepositoryName \"Arcade\" " +
                                            "-TsaCodebaseName \"Arcade\" " +
                                            "-TsaPublish $True" },
                            }
                        }
                    }),
                }
        };
    }
}
