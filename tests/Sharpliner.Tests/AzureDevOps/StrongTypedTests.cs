using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class StrongTypedTests
{
    private class Pipeline_With_Strong_Variables_And_Parameters : SimpleTestPipeline
    {
        private static readonly Parameter s_parameter1 = StringParameter("Parameter1", "SomeParameterValue1");
        private static readonly Parameter s_parameter2 = StringParameter("Parameter2", "SomeParameterValue2");

        private static readonly Variable s_variable1 = new("Variable1", "SomeVariableValue1");
        private static readonly Variable s_variable2 = new("Variable2", "SomeVariableValue2");
        private static readonly Variable s_variableBasedUponParameter = new("VariableBasedUponParameter", string.Empty);

        public override SingleStagePipeline Pipeline { get; } = new()
        {
            Variables =
            {
                s_variable1,
                s_variable2,
                If.Equal(s_parameter1, "SomeParameterValue1")
                    .Variable(s_variableBasedUponParameter with { Value = "Parameter1 Equals SomeParameterValue1" })
                    .Else
                    .Variable(s_variableBasedUponParameter with { Value =  "Parameter1 Does Not Equal SomeParameterValue1" })
            },
            Parameters =
            {
                s_parameter1,
                s_parameter2
            },
            Jobs =
            {
                new Job("Blah", "Blah")
                {
                    Steps =
                    {
                        If.Equal(s_parameter1, "SomeParameterValue1")
                            .Step(Script.Inline("echo Hello")),

                        If.Equal(s_variableBasedUponParameter, "Parameter1 Equals SomeParameterValue1")
                            .Step(Script.Inline("echo Hello again")),

                        Script.Inline($"echo {s_variableBasedUponParameter}"),

                        Script.Inline($"echo {s_parameter1}"),
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
