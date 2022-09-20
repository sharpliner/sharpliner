using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class StrongTypedTests
{
    private class Pipeline_With_Strong_Variables_And_Parameters : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline { get; } = new()
        {
            Variables =
            {
                Variable("Variable1", "SomeVariableValue1"),

                Variable("Variable2", "SomeVariableValue2"),

                If.Equal(parameters["Parameter1"], "SomeParameterValue1")
                    .Variable("VariableBasedUponParameter", "Parameter1 Equals SomeParameterValue1")
                    .Else
                    .Variable("VariableBasedUponParameter", "Parameter1 Does Not Equal SomeParameterValue1")
            },
            Parameters =
            {
                StringParameter("Parameter1", defaultValue: "SomeParameterValue1"),
                StringParameter("Parameter2", defaultValue: "SomeParameterValue2")
            },
            Jobs =
            {
                new Job("Blah", "Blah")
                {
                    Steps =
                    {
                        If.Equal(parameters["Parameter1"], "SomeParameterValue1")
                            .Step(Script.Inline("echo Hello")),

                        If.Equal(staticVariables["VariableBasedUponParameter"], "Parameter1 Equals SomeParameterValue1")
                            .Step(Script.Inline("echo Hello again")),

                        Script.Inline($"echo {variables["VariableBasedUponParameter"]}"),

                        Script.Inline($"echo {parameters["Parameter1"]}"),
                    }
                },
            }
        };
    }

    [Fact]
    public void Variable_And_Parameter_Serialize_Differently_Within_Scripts_And_Conditions_Test()
    {
        var yaml = new Pipeline_With_Strong_Variables_And_Parameters().Serialize();

        yaml.Trim().Should().Be("""
            parameters:
            - name: Parameter1
              type: string
              default: SomeParameterValue1

            - name: Parameter2
              type: string
              default: SomeParameterValue2

            variables:
            - name: Variable1
              value: SomeVariableValue1

            - name: Variable2
              value: SomeVariableValue2

            - ${{ if eq(parameters.Parameter1, 'SomeParameterValue1') }}:
              - name: VariableBasedUponParameter
                value: Parameter1 Equals SomeParameterValue1

            - ${{ else }}:
              - name: VariableBasedUponParameter
                value: Parameter1 Does Not Equal SomeParameterValue1

            jobs:
            - job: Blah
              displayName: Blah
              steps:
              - ${{ if eq(parameters.Parameter1, 'SomeParameterValue1') }}:
                - script: |-
                    echo Hello

              - ${{ if eq(variables['VariableBasedUponParameter'], 'Parameter1 Equals SomeParameterValue1') }}:
                - script: |-
                    echo Hello again

              - script: |-
                  echo $(VariableBasedUponParameter)

              - script: |-
                  echo ${{ parameters.Parameter1 }}
            """
        );
    }
}
