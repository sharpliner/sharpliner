using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.Tests.AzureDevOps;

public class JobDependencyVariableReferenceTests
{
    private class JobDependencyVariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private string setterJobName = "Setter_Job";
        private string setterTaskName = "VarSetter";
        private string sharedVariableName = "MyVar";

        private string getterJobName = "Getter_Job";

        public override ConditionedList<Stage> Definition =>
        [
            new Stage("Stage_1")
            {
                Jobs =
                {
                    new Job(setterJobName)
                    {
                        Pool = variables["pool"],

                        Steps =
                        {
                            Bash.Inline($"echo ##vso[task.setvariable variable={sharedVariableName};isOutput=true]true") with
                            {
                                Name = setterTaskName
                            }
                        }
                    },
                    new Job(getterJobName)
                    {
                        Pool = variables["pool"],

                        Steps =
                        {
                            Bash.Inline("echo This should be run!")
                        },
                        Condition = Equal(new JobDependencyVariable(sharedVariableName, setterTaskName, setterJobName), "true")
                    },
                }
            }
        ];
    }

    // This test should create a 
    [Fact]
    public Task PipelineVariable_Serialization_Test()
    {
        var pipeline = new JobDependencyVariableReferenceTest_Template();

        return Verify(pipeline.Serialize());
    }
}
