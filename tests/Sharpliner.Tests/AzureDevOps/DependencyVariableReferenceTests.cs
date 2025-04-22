using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DependencyVariableReferenceTests
{
    /// <summary>
    /// Test the <see cref="StageToStageDependencyVariableReference"/> output.  Adapated from the 2nd example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#stage-to-stage-dependencies">seen here</a>.
    /// </summary>
    private class StageToStageDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterStageName = "A";
        private string setterJobName = "A1";
        private string setterStepName = "printvar";
        private string sharedVariableName = "shouldrun";

        private string getterStageName = "B";
        private string getterJobName = "B1";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage(setterStageName)
            {
                Jobs =
                {
                    new Job(setterJobName)
                    {
                        Steps =
                        {
                            Bash.Inline($"echo \"##vso[task.setvariable variable={sharedVariableName};isOutput=true]true\"") with
                            {
                                Name = setterStepName
                            }
                        }
                    },
                }
            },
            new Stage(getterStageName)
            {
                Condition = And(InlineCondition.Succeeded, Equal(dependencies.stage[setterStageName, setterJobName, setterStepName, sharedVariableName], "true")),
                DependsOn = setterStageName,
                Jobs =
                {
                    new Job(getterJobName)
                    {
                        Steps =
                        {
                            Script.Inline($"echo hello from Stage {getterStageName}")
                        }
                    },
                }
            }
        ];
    }

    /// <summary>
    /// Test the <see cref="JobToJobSameStageDependencyVariableReference"/> output.  Adapated from the 2nd example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#job-to-job-dependencies-within-one-stage">seen here</a>.
    /// </summary>
    private class JobToJobSameStageDependencyVariableReferenceTest_Template : JobTemplateDefinition
    {
        public override string TargetFile => "jobs.yml";

        private string setterJobName = "A";
        private string setterStepName = "printvar";
        private string sharedVariableName = "shouldrun";

        private string getterJobName = "B";

        public override ConditionedList<JobBase> Definition =>
        [
            new Job(setterJobName)
            {
                Steps =
                {
                    Bash.Inline($"echo \"##vso[task.setvariable variable={sharedVariableName};isOutput=true]true\"") with
                    {
                        Name = setterStepName
                    }
                }
            },
            new Job(getterJobName)
            {
                DependsOn = setterJobName,
                Condition = And(InlineAndCondition.Succeeded, Equal(dependencies.job[setterJobName, setterStepName, sharedVariableName], "true")),
                Steps =
                {
                    Script.Inline($"echo hello from {getterJobName}")
                }
            }
        ];
    }

    /// <summary>
    /// Test the <see cref="JobToJobDifferentStageDependencyVariableReference"/> output.  Adapated from the 1st example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#job-to-job-dependencies-across-stages">seen here</a>.
    /// </summary>
    private class JobToJobDifferentStageDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterStageName = "A";
        private string setterJobName = "A1";
        private string setterStepName = "printvar";
        private string sharedVariableName = "shouldrun";

        private string getterStageName = "B";
        private string getterJobName = "B1";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage(setterStageName)
            {
                Jobs =
                {
                    new Job(setterJobName)
                    {
                        Steps =
                        {
                            Bash.Inline($"echo \"##vso[task.setvariable variable={sharedVariableName};isOutput=true]true\"") with
                            {
                                Name = setterStepName
                            }
                        }
                    },
                }
            },
            new Stage(getterStageName)
            {
                DependsOn = setterStageName,
                Jobs =
                {
                    new Job(getterJobName)
                    {
                        Condition = Equal(dependencies.job[setterStageName, setterJobName, setterStepName, sharedVariableName], "true"),
                        Steps =
                        {
                            Script.Inline($"echo hello from Job {getterJobName}")
                        }
                    },
                }
            }
        ];
    }

    /// <summary>
    /// Test the <see cref="JobToJobDeployDependencyVariableReference"/> output.  Adapated from the 2nd example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#job-to-job-dependencies-across-stages">seen here</a>.
    /// </summary>
    private class JobToJobDeployDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterStageName = "build";
        private string setterJobName = "build_job";
        private string setterStepName = "setRunTests";
        private string sharedVariableName = "runTests";

        private string getterStageName = "test";
        private string getterJobName = "run_tests";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage(setterStageName)
            {
                Jobs =
                {
                    new DeploymentJob(setterJobName)
                    {
                        Environment = new Sharpliner.AzureDevOps.Environment("Production"),
                        Strategy = new RunOnceStrategy
                        {
                            Deploy = new()
                            {
                                Steps =
                                [
                                    new PwshTaskBuilder().Inline(
                                        $"""
                                        ${sharedVariableName} = "true"
                                        echo "setting {sharedVariableName}: ${sharedVariableName}"
                                        echo "##vso[task.setvariable variable={sharedVariableName};isOutput=true]${sharedVariableName}"
                                        """
                                    ) with
                                    {
                                        Name = setterStepName
                                    }
                                ]
                            }
                        }
                    },
                }
            },
#region dependency-variables
            new Stage(getterStageName) {
                DependsOn = setterStageName,
                Jobs =
                {
                    new Job(getterJobName)
                    {
                        Condition = Equal(dependencies.job.deploy[setterStageName, setterJobName, setterStepName, sharedVariableName], "true"),
                        Steps =
                        {
                            Script.Inline($"echo hello from Job {getterJobName}")
                        }
                    },
                }
            }
#endregion
        ];
    }

    /// <summary>
    /// Test the <see cref="StageToStageDeployDependencyVariableReference"/> output.  Adapated from the 1st example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#deployment-job-output-variables">seen here</a>.
    /// </summary>
    private class StageToStageDeployDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterStageName = "build";
        private string setterJobName = "build_job";
        private string setterStepName = "setRunTests";
        private string sharedVariableName = "runTests";

        private string getterStageName = "test";
        private string getterJobName = "A";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage(setterStageName)
            {
                Jobs =
                {
                    new DeploymentJob(setterJobName)
                    {
                        Environment = new Sharpliner.AzureDevOps.Environment("Production"),
                        Strategy = new RunOnceStrategy
                        {
                            Deploy = new()
                            {
                                Steps =
                                [
                                    new PwshTaskBuilder().Inline(
                                        $"""
                                        ${sharedVariableName} = "true"
                                        echo "setting {sharedVariableName}: ${sharedVariableName}"
                                        echo "##vso[task.setvariable variable={sharedVariableName};isOutput=true]${sharedVariableName}"
                                        """
                                    ) with
                                    {
                                        Name = setterStepName
                                    }
                                ]
                            }
                        }
                    },
                }
            },
            new Stage(getterStageName) {
                Condition = Equal(dependencies.stage.deploy[setterStageName, setterJobName, setterStepName, sharedVariableName], "true"),
                DependsOn = setterStageName,
                Jobs =
                {
                    new Job(getterJobName)
                    {
                        Steps =
                        {
                            Script.Inline($"echo hello from Job {getterJobName}")
                        }
                    },
                }
            }
        ];
    }

    /// <summary>
    /// Test the <see cref="StageToStageDeployDependencyVariableReference"/> output.  Adapated from the 2nd example <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#deployment-job-output-variables">seen here</a>.
    /// </summary>
    private class StageToStageDeployResourceDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterStageName = "build";
        private string setterJobName = "build_job";
        private string setterStepName = "setRunTests";
        private string setterResourceName = "winVM2";
        private string sharedVariableName = "runTests";

        private string getterStageName = "test";
        private string getterJobName = "A";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage(setterStageName)
            {
                Jobs =
                {
                    new DeploymentJob(setterJobName)
                    {
                        Environment = new Sharpliner.AzureDevOps.Environment("vmtest", setterResourceName)
                        {
                            ResourceType = ResourceType.VirtualMachine
                        },
                        Strategy = new RunOnceStrategy
                        {
                            Deploy = new()
                            {
                                Steps =
                                [
                                    new PwshTaskBuilder().Inline(
                                        $"""
                                        ${sharedVariableName} = "true"
                                        echo "setting {sharedVariableName}: ${sharedVariableName}"
                                        echo "##vso[task.setvariable variable={sharedVariableName};isOutput=true]${sharedVariableName}"
                                        """
                                    ) with
                                    {
                                        Name = setterStepName
                                    }
                                ]
                            }
                        }
                    },
                }
            },
            new Stage(getterStageName) {
                Condition = Equal(dependencies.stage.deploy[setterStageName, setterJobName, setterStepName, sharedVariableName, setterResourceName], "true"),
                DependsOn = setterStageName,
                Jobs =
                {
                    new Job(getterJobName)
                    {
                        Steps =
                        {
                            Script.Inline($"echo hello from Job {getterJobName}")
                        }
                    },
                }
            }
        ];
    }

    [Fact]
    public Task StageToStageDependencyVariable_Serialization_Test()
    {
        var pipeline = new StageToStageDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task JobToJobSameStageDependencyVariable_Serialization_Test()
    {
        var pipeline = new JobToJobSameStageDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task JobToJobDifferentStageDependencyVariable_Serialization_Test()
    {
        var pipeline = new JobToJobDifferentStageDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task JobToJobDeployDependencyVariable_Serialization_Test()
    {
        var pipeline = new JobToJobDeployDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task StageToStageDeployDependencyVariable_Serialization_Test()
    {
        var pipeline = new StageToStageDeployDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }

    [Fact]
    public Task StageToStageDeployResourceDependencyVariable_Serialization_Test()
    {
        var pipeline = new StageToStageDeployResourceDependencyVariableReferenceTest_Template();
        return Verify(pipeline.Serialize());
    }
}
