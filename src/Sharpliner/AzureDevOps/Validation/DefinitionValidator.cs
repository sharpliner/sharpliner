using System.Text.RegularExpressions;

namespace Sharpliner.AzureDevOps.Validation;

internal static class DefinitionValidator
{
    internal static readonly Regex NameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    public static void Validate(PipelineBase definition)
    {
        var validations = definition.Validations;

        foreach (var validation in validations)
        {
            validation.Validate();
        }
    }
}
