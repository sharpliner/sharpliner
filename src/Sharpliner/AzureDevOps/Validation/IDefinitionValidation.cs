using System;

namespace Sharpliner.AzureDevOps.Validation;

internal interface IDefinitionValidation
{
    void Validate();

    ValidationSeverity SeveritySetings { get; }
}

public enum ValidationSeverity
{
    Off = 0,
    Trace = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
}

internal class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
