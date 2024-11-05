using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.Tests.AzureDevOps;

/// <summary>
/// Definition of https://github.com/dotnet/xharness/blob/main/azure-pipelines.yml (not 100%)
/// Used for testing of API inside of this repository
/// </summary>
internal class XHarnessPipeline : ExtendsPipelineDefinition
{
    public override string TargetFile => "dotnet/xharness/azure-pipelines.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    KeyValuePair<string, object> teamName = new("_TeamName", "DotNetCore");

    public override PipelineWithExtends Pipeline => new()
    {
        Variables =
        {
            VariableTemplate<CommonVariables>(),
        },

        Trigger = new Trigger("main", "release/*", "internal/release/*")
        {
            Batch = true
        },

        Pr = PrTrigger.None,

        Resources = new Resources
        {
            Repositories =
            [
                new RepositoryResource("1ESPipelineTemplates") with
                {
                    Type = RepositoryType.Git,
                    Name = "1ESPipelineTemplates/1ESPipelineTemplates",
                    Ref = "refs/tags/release",
                }
            ]
        },

        Extends = new Extends("v1/1ES.Official.PipelineTemplate.yml@1ESPipelineTemplates", new TemplateParameters
        {
            // sdl
            // pool
            ["stages"] = new ConditionedList<Stage>
            {
                new Stage("Build")
                {
                    Jobs =
                    {
                        JobTemplate("/eng/common/templates-official/jobs/jobs.yml", new TemplateParameters
                        {
                            ["enableTelemetry"] = true,
                            ["enablePublishBuildArtifacts"] = true,
                            ["enableMicrobuild"] = true,
                            ["enablePublishUsingPipelines"] = true,
                            ["enablePublishBuildAssets"] = true,
                            ["helixRepo"] = "dotnet/xharness",
                            ["jobs"] = new ConditionedList<Job>
                            {
                                new Job("Windows_NT", "Build Windows")
                                {
                                    Steps =
                                    {
                                        (Script.Inline($"eng\\common\\CIBuild.cmd -configuration {CommonVariables.BuildConfig} -prepareMachine {CommonVariables.InternalBuildArgs} /p:Test=false") with
                                        {
                                            Name = "Build",
                                            DisplayName = "Build"
                                        }).WhenSucceeded()
                                    }
                                }
                            }
                        }),
                        JobTemplate<CommonPostBuild, CommonPostBuildParameters>(new()
                        {
                            EnableSymbolValidation = true,
                            EnableSourceLinkValidation = true
                        })
                    }
                }
            }
        })
    };

    private class CommonVariables : VariableTemplateDefinition
    {
        public static Variable TeamName { get; } = new("_TeamName", "DotNetCore");
        public static Variable HelixApiAccessToken { get; } = new("HelixApiAccessToken", string.Empty);
        public static Variable InternalBuildArgs { get; } = new("_InternalBuildArgs", string.Empty);

        public static Variable SignType { get; } = new("_SignType", "real");
        public static Variable BuildConfig { get; } = new("_BuildConfig", "release");

        public static VariableGroup PublishBuildAssets { get; } = new("Publish-Build-Assets");
        public static VariableGroup DotNetHelixApiAccess { get; } = new("DotNet-HelixApi-Access");
        public static VariableGroup SdlSettings { get; } = new("SDL_Settings");

        public override string TargetFile => "eng/common-variables.yml";
        public override ConditionedList<VariableBase> Definition =>
        [
            TeamName,
            HelixApiAccessToken,
            InternalBuildArgs,
            If.And(
                NotEqual(variables.System.TeamProject, "public"),
                IsNotPullRequest
            ).Variables(
                SignType,
                BuildConfig,
                PublishBuildAssets,
                DotNetHelixApiAccess,
                SdlSettings,
                Variable(InternalBuildArgs.Name,
                $"""
                /p:DotNetSignType={SignType}
                /p:TeamName={TeamName}
                /p:DotNetPublishUsingPipelines=true
                /p:OfficialBuildId={variables.Build.BuildId}
                """)
            ),
        ];
    }

    private class CommonPostBuild : JobTemplateDefinition<CommonPostBuildParameters>
    {
        public override string TargetFile => "eng/common/templates-official/post-build/post-build.yml";
        public override ConditionedList<JobBase> Definition =>
        [
        ];
    }

    private class CommonPostBuildParameters : CorePostBuildParameters
    {
        public override bool Is1ESPipeline { get; set; } = false;
    }

    private class CorePostBuild : StageTemplateDefinition<CorePostBuildParameters>
    {
        public override string TargetFile => "eng/common/core-templates/post-build/post-build.yml";
        public override ConditionedList<Stage> Definition =>
        [
            If.Or(
                Equal(TemplateParameters.EnableNugetValidation, "true"),
                Equal(TemplateParameters.EnableSigningValidation, "true"),
                Equal(TemplateParameters.EnableSourceLinkValidation, "true"),
                Equal(TemplateParameters.SDLValidationParameters.Enable, "true")
            ).Stage(new("Validate", "Validate Build Assets")
            {
                DependsOn = TemplateParameters.ValidateDependsOn,
                Variables =
                {
                    Template("/eng/common/core-templates/post-build/common-variables.yml"),
                    new VariableTemplate("/eng/common/core-templates/variables/pool-providers.yml")
                    {
                        Parameters = new()
                        {
                            ["is1ESPipeline"] = TemplateParameters.Is1ESPipeline
                        }
                    }
                }
            })
        ];
    }

    private class CorePostBuildParameters : TemplateParametersProviderBase<CorePostBuildParameters>
    {
        public virtual int PublishingInfraVersion { get; set; } = 3;

        public virtual int BARBuildId { get; set; } = 0;

        public virtual string PromoteToChannelIds { get; set; } = string.Empty;

        public virtual bool EnableSourceLinkValidation { get; set; } = true;

        public virtual bool EnableSigningValidation { get; set; } = false;

        public virtual bool EnableSymbolValidation { get; set; }

        public virtual bool EnableNugetValidation { get; set; } = true;

        public virtual bool PublishInstallersAndChecksums { get; set; } = true;

        public virtual SDLValidationParameters SDLValidationParameters { get; set; }

        public virtual string SymbolPublishingAdditionalParameters { get; set; } = string.Empty;

        public virtual string ArtifactsPublishingAdditionalParameters { get; set; } = string.Empty;

        public virtual string SigningValidationAdditionalParameters { get; set; } = string.Empty;

        public virtual List<string> ValidateDependsOn { get; set; } = ["build"];

        public virtual List<string> PublishDependsOn { get; set; } = ["Validate"];

        public virtual bool PublishAssetsImmediately { get; set; } = false;

        public virtual bool Is1ESPipeline { get; set; }
    }

    public class SDLValidationParameters
    {
        public bool Enable { get; set; } = false;
        public bool PublishGdn { get; set; } = false;
        public bool ContinueOnError { get; set; } = false;
        public string Params { get; set; } = string.Empty;
        public string ArtifactNames { get; set; } = string.Empty;
        public bool DownloadArtifacts { get; set; } = true;
    }
}
