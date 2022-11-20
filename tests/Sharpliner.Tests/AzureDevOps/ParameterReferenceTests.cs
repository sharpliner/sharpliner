using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.Tests.AzureDevOps;

public class ParameterReferenceTests
{
    private class RunOnWindowsTemplate : StepTemplateDefinition
    {
        public override string TargetFile => "run-on-windows.yml";

        public override List<Parameter> Parameters => new()
        {
            StringParameter("agentOs"),
            StepListParameter("steps"),
        };

            public override ConditionedList<Step> Definition => new()
        {
            If.Equal(parameters["agentOs"], "Windows_NT")
                .Step(parameters["steps"])
        };
    }
}
