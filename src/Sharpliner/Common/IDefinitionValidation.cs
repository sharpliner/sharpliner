using System.Collections.Generic;

namespace Sharpliner.Common;

public interface IDefinitionValidation
{
    IReadOnlyCollection<ValidationError> Validate();
}

public enum ValidationSeverity
{
    Off = 0,
    Trace = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
}

public class ValidationError(ValidationSeverity severity, string message)
{
    public ValidationSeverity Severity { get; } = severity;

    public string Message { get; } = message;
}
