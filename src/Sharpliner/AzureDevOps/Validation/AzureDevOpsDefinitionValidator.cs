using System.Text.RegularExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

internal static class AzureDevOpsDefinitionValidator
{
    internal static readonly Regex NameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    public static void Validate(PipelineBase definition)
    {
        var validations = definition.Validations;

        foreach (var validation in validations)
        {
            if (validation.SeveritySetings == ValidationSeverity.Off)
            {
                continue;
            }

            validation.Validate();
        }
    }
}
