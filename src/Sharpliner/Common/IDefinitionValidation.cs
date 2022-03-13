using System.Collections.Generic;
using System.Linq;

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

public class ValidationError
{
    public ValidationSeverity Severity { get; }

    public string Message { get; }

    public ValidationError(ValidationSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
    }
}
