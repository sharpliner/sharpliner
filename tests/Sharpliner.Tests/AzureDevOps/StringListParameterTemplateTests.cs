using System.Collections.Generic;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps;

public class StringListParameterTemplateTests
{
    /// <summary>
    /// Test template definition that uses IEnumerable&lt;string&gt; in typed parameters.
    /// This should generate 'object' type parameters, not 'stringList', as per Azure DevOps documentation.
    /// </summary>
    private class TemplateWithStringList : StepTemplateDefinition<TemplateWithStringListParameters>
    {
        public override string TargetFile => "template-with-string-list.yml";

        public override AdoExpressionList<Step> Definition =>
        [
            Bash.Inline("echo 'Processing environments'"),
        ];
    }

    private class TemplateWithStringListParameters
    {
        public IEnumerable<string> Environments { get; init; } = ["dev", "staging", "prod"];
        public string[] Tags { get; init; } = ["build", "test"];
        public List<string> Platforms { get; init; } = ["x64", "ARM64"];
    }

    [Fact]
    public Task Template_With_StringList_Should_Use_Object_Type()
    {
        var template = new TemplateWithStringList();
        var serialized = template.Serialize();
        
        return Verify(serialized);
    }
}