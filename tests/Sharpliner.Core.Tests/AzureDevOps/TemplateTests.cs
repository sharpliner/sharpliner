using System.ComponentModel;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.Tests.AzureDevOps;

public class TemplateTests
{
    private class TemplateList_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.Equal(parameters["restore"], "bar")
                    .JobTemplate("template1.yml", new TemplateParameters
                    {
                        { "enableTelemetry", true },
                    })
                    .JobTemplate("template2.yml", new TemplateParameters
                    {
                        { "enableTelemetry", false },
                    })
            }
        };
    }

    [Fact]
    public Task TemplateList_Serialization_Test()
    {
        var pipeline = new TemplateList_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Extends_Template_Definition : ExtendsTemplateDefinition
    {
        public override string TargetFile => "extends-template.yml";
        public override List<Parameter> Parameters =>
        [
            StringParameter("some", defaultValue: "default value"),
            BooleanParameter("other", defaultValue: true),
        ];

        public override Extends Definition => new("template.yml", new()
        {
            ["some"] = "value",
            ["other"] = parameters["other"],
        });
    }

    [Fact]
    public Task Extends_Template_Definition_Serialization_Test()
    {
        var pipeline = new Extends_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Extends_Typed_Template_Definition(ExtendsTypedParameters? parameters = null)
        : ExtendsTemplateDefinition<ExtendsTypedParameters>(parameters)
    {
        public override string TargetFile => "extends-typed-template.yml";
        public override Extends Definition => new("template.yml", new()
        {
            ["some"] = "value",
            ["other"] = parameters["other"],
        });
    }

    class ExtendsTypedParameters
    {
        public string Some { get; init; } = "default value";
        [DisplayName("The other parameter")]
        public bool Other { get; init; } = true;
    }

    [Fact]
    public Task Extends_Typed_Template_Definition_Serialization_Test()
    {
        var pipeline = new Extends_Typed_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Step_Template_Definition : StepTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        protected Parameter configuration = EnumParameter<BuildConfiguration>("configuration", defaultValue: BuildConfiguration.Debug);
        protected Parameter project = StringParameter("project");
        protected Parameter version = StringParameter("version", allowedValues: [ "5.0.100", "5.0.102" ]);
        protected Parameter skipBuild = BooleanParameter("skipBuild");
        protected Parameter useNugetOrg = BooleanParameter("useNugetOrg", defaultValue: false);
        protected Parameter restore = BooleanParameter("restore", defaultValue: true);
        protected Parameter<Step> afterBuild = StepParameter("afterBuild", defaultValue: Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}"));

        public override List<Parameter> Parameters =>
        [
            configuration,
            project,
            version,
            skipBuild,
            useNugetOrg,
            restore,
            afterBuild,
        ];

        public override AdoExpressionList<Step> Definition =>
        [
            DotNet.Install.Sdk(version),

            If.Equal(restore, "true")
                .Step(DotNet.Restore.Projects(project)),

            DotNet.Build(project),

            StepParameterReference(afterBuild),
        ];
    }

    private enum BuildConfiguration
    {
        [YamlMember(Alias = "debug")]
        Debug,

        [YamlMember(Alias = "release")]
        Release,
    }

    [Fact]
    public Task Step_Template_Definition_Serialization_Test()
    {
        var pipeline = new Step_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Step_Typed_Template_Definition(StepTypedParameters? parameters = null)
        : StepTemplateDefinition<StepTypedParameters>(parameters)
    {
        public override string TargetFile => "step-template.yml";

        public override AdoExpressionList<Step> Definition => new Step_Template_Definition().Definition;
    }

    class StepTypedParameters : AzureDevOpsDefinition
    {
        public BuildConfiguration Configuration { get; init; }

        public string? Project { get; init; }

        [AllowedValues("5.0.100", "5.0.102")]
        public string? Version { get; init; }

        public bool? SkipBuild { get; init; }

        public bool UseNugetOrg { get; init; } = false;

        public bool Restore { get; init; } = true;

        public Step AfterBuild { get; init; } = Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}");

        [YamlMember(Alias = "theCounter")]
        public int Counter { get; init; } = 2;

        [AllowedValues(1, 2, 3, 4)]
        public int? DefaultCounter { get; init; }

        #region Parameters with empty default values
        public object? ObjectEmptyByDefault { get; init; } = new();
        public string[]? ArrayEmptyByDefault { get; init; } = [];
        public string[]? ArrayWithDefaults { get; init; } = ["foo", "bar"];
        #endregion
    }

    [Fact]
    public Task Step_Typed_Template_Definition_Serialization_Test()
    {
        var pipeline = new Step_Typed_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Job_Template_Definition : JobTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        JobParameter mainJob = JobParameter("mainJob");

        public override List<Parameter> Parameters =>
        [
            mainJob,
        ];

        public override AdoExpressionList<JobBase> Definition =>
        [
            Job("initialize") with
            {
                DisplayName = "Initialize job",
            },
            JobParameterReference(mainJob),
            Job("finalize") with
            {
                DisplayName = "Finalize job",
            },
        ];
    }
    
    [Fact]
    public Task Job_Template_Definition_Serialization_Test()
    {
         var pipeline = new Job_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Job_Typed_Template_Definition(JobTypedParameters? parameters = null)
        : JobTemplateDefinition<JobTypedParameters>(parameters)
    {
        public override string TargetFile => "job-template.yml";

        public override AdoExpressionList<JobBase> Definition =>
        [
            ..new Job_Template_Definition().Definition,
            Job("with-templates") with
            {
                Steps =
                [
                    new Step_Typed_Template_Definition(new()
                    {
                        AfterBuild = Bash.Inline("echo 'After build'"),
                        Counter = 3,
                        UseNugetOrg = true
                    })
                ]
            }
        ];
    }

    private class Job_Typed_Template_Relative_To_Git_Definition(JobTypedParameters? parameters = null) : Job_Typed_Template_Definition(parameters)
    {
        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
        public override string TargetFile => "job-template.yml";

        public override AdoExpressionList<JobBase> Definition =>
        [
            ..new Job_Template_Definition().Definition,
            Job("with-templates") with
            {
                Steps =
                [
                    new Step_Typed_Template_Definition(new()
                    {
                        AfterBuild = Bash.Inline("echo 'After build'"),
                        Counter = 3,
                        UseNugetOrg = true
                    })
                ]
            }
        ];
    }

    class JobTypedParameters : AzureDevOpsDefinition
    {
        public AdoExpressionList<JobBase> SetupJobs { get; init; } = [];

        public JobBase MainJob { get; init; } = null!;

        public DeploymentJob Deployment { get; init; } = new("deploy", "Deploy job")
        {
            Environment = new("production")
            {
                Tags = ["group-a", "group-b"]
            },
            Strategy = new RunOnceStrategy
            {
                Deploy = new()
                {
                    Steps =
                    {
                        Bash.Inline("echo 'Deploying the application'"),
                    },
                },
            }
        };

        public AdoExpressionList<DeploymentJob> AdditionalDeployments { get; init; } = [];
    }

    [Fact]
    public Task Job_Typed_Template_Definition_Serialization_Test()
    {
         var pipeline = new Job_Typed_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Stage_Template_Definition : StageTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        Parameter<Stage> mainStage = StageParameter("mainStage");

        public override List<Parameter> Parameters =>
        [
            mainStage,
        ];

        public override AdoExpressionList<Stage> Definition =>
        [
            Stage("initialize") with
            {
                DisplayName = "Initialize stage",
            },
            StageParameterReference(mainStage),
            Stage("finalize") with
            {
                DisplayName = "Finalize stage",
            },
        ];
    }

    [Fact]
    public Task Stage_Template_Definition_Serialization_Test()
    {
        var pipeline = new Stage_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Stage_Typed_Template_Definition(StageTypedParameters? parameters = null)
        : StageTemplateDefinition<StageTypedParameters>(parameters)
    {
        public override string TargetFile => "stage-template.yml";

        public override AdoExpressionList<Stage> Definition =>
        [
            ..new Stage_Template_Definition().Definition,
            Stage("with-templates") with
            {
                Jobs =
                [
                    new Job_Typed_Template_Definition(new()
                    {
                        MainJob = new Job("main", "Main job")
                        {
                            Steps =
                            [
                                Bash.Inline("echo 'Main job step'")
                            ]
                        }
                    })
                ]
            }
        ];
    }

    private class Stage_Typed_Template_With_Job_Template_Relative_To_Git_Definition(StageTypedParameters? parameters = null)
        : StageTemplateDefinition<StageTypedParameters>(parameters)
    {
        public override string TargetFile => "stage-template.yml";

        public override AdoExpressionList<Stage> Definition =>
        [
            ..new Stage_Template_Definition().Definition,
            Stage("with-templates") with
            {
                Jobs =
                [
                    new Job_Typed_Template_Relative_To_Git_Definition(new()
                    {
                        MainJob = new Job("main", "Main job")
                        {
                            Steps =
                            [
                                Bash.Inline("echo 'Main job step'")
                            ]
                        }
                    })
                ]
            }
        ];
    }

    private class Stage_Typed_Template_Relative_To_Git_Definition(StageTypedParameters? parameters = null) : Stage_Typed_Template_Definition(parameters)
    {
        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
    }

    class StageTypedParameters : AzureDevOpsDefinition
    {
        public AdoExpressionList<Stage> SetupStages { get; init; } = [];

        public Stage MainStage { get; init; } = null!;
    }

    [Fact]
    public Task Stage_Typed_Template_Definition_Serialization_Test()
    {
        var pipeline = new Stage_Typed_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task Stage_Typed_Template_Definition_Relative_To_Git_Serialization_Test()
    {
        var pipeline = new Stage_Typed_Template_With_Job_Template_Relative_To_Git_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Variable_Template_Definition : VariableTemplateDefinition
    {
        public override string TargetFile => "variables.yml";

        public override List<Parameter> Parameters =>
        [
          StringParameter("s_param"),
          BooleanParameter("b_param"),
          NumberParameter("n_param"),
        ];

        public override AdoExpressionList<VariableBase> Definition =>
        [
          Variable("s_variable", "value"),
          Variable("b_variable", true),
          Variable("n_variable", 42),
        ];
    }

    [Fact]
    public Task Variable_Template_Definition_Serialization_Test()
    {
        var pipeline = new Variable_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class Variable_Typed_Template_Definition(VariableTypedParameters? parameters = null)
        : VariableTemplateDefinition<VariableTypedParameters>(parameters)
    {
        public override string TargetFile => "variables.yml";
        public override AdoExpressionList<VariableBase> Definition => new Variable_Template_Definition().Definition;
    }

    class VariableTypedParameters
    {
        [YamlMember(Alias = "s_param")]
        public string SParam { get; init; } = "default value";
        [YamlMember(Alias = "b_param")]
        public bool BParam { get; init; } = true;
        [YamlMember(Alias = "n_param")]
        public int NParam { get; init; } = 42;
    }

    [Fact]
    public Task Variable_Typed_Template_Definition_Serialization_Test()
    {
        var pipeline = new Variable_Typed_Template_Definition();

        return Verify(pipeline.Serialize());
    }

    private class CompletePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            [
                new Stage_Typed_Template_Definition(new()
                {
                    MainStage = new Stage("main-stage")
                    {
                        Jobs =
                        [
                            new Job_Typed_Template_Definition(new()
                            {
                                MainJob = new Job("main-job", "Main job")
                                {
                                    Steps =
                                    [
                                        Bash.Inline("echo 'Hello world!'"),
                                        new Step_Typed_Template_Definition(new()
                                        {
                                            AfterBuild = Bash.Inline("echo 'After build'"),
                                            Counter = 3,
                                            UseNugetOrg = true
                                        })
                                    ]
                                }
                            })
                        ]
                    }
                }),
            ]
        };
    }

    [Fact]
    public Task CompletePipeline_Serialization_Test()
    {
        var pipeline = new CompletePipeline();

        return Verify(pipeline.Serialize());
    }

    private class Expression_Template_Reference : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            If.Equal("restore", "true")
                .StepTemplate("template1.yaml"),

            If.IsBranch("main")
                .StepTemplate("template2.yaml")
                .If.IsPullRequest
                    .StepTemplate("template3.yaml"),
        ];
    }

    [Fact]
    public Task Expression_Template_Reference_Serialization_Test()
    {
        var pipeline = new Expression_Template_Reference();

        return Verify(pipeline.Serialize());
    }

    private class Expression_Parameters : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            StepTemplate("template1.yaml", new()
            {
                { "some", "value" },
                {
                    If.IsPullRequest,
                    new TemplateParameters()
                    {
                        { "pr", true }
                    }
                },
                {
                    "other",
                    If.Equal(parameters["container"], "")
                        .Value(new TemplateParameters
                        {
                            { "image", "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be" }
                        })
                    .Else
                        .Value(new TemplateParameters
                        {
                            { "image", parameters["container"] }
                        })
                },
            }),
        ];
    }

    [Fact]
    public Task Expression_Parameters_Serialization_Test()
    {
        var pipeline = new Expression_Parameters();

        return Verify(pipeline.Serialize());
    }

    private class Expression_Indexers_Parameters : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            StepTemplate("template1.yaml", new()
            {
                ["some"] = "value",
                [If.IsPullRequest] = new TemplateParameters()
                {
                    ["pr"] = true
                },
                ["other"] =
                    If.Equal(parameters["container"], "")
                        .Value(new TemplateParameters
                        {
                            ["image"] = "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be"
                        })
                    .Else
                        .Value(new TemplateParameters
                        {
                            ["image"] = parameters["container"]
                        })
            }),
        ];
    }

    [Fact]
    public Task Expression_Indexers_Parameters_Serialization_Test()
    {
        var pipeline = new Expression_Indexers_Parameters();

        return Verify(pipeline.Serialize());
    }

    private class Expression_Template_Variables : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Variables = CreateVariables(),
        };

        private static AdoExpressionList<VariableBase> CreateVariables()
        {
            var items = new AdoExpressionList<VariableBase>
            {
                new Variable("some", "value"),
                If.IsPullRequest.Variable("pr", true).Else.Variable("pr", false),
            };
            items[1] =
                If.Equal(parameters["container"], "")
                    .Variable("image", "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be")
                .Else
                    .Variable("image", parameters["container"]);
            return items;
        }
    }

    [Fact]
    public Task Expression_Template_Variables_Serialization_Test()
    {
        var pipeline = new Expression_Template_Variables();

        return Verify(pipeline.Serialize());
    }
}
