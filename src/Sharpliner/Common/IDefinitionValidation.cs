using System.Collections.Generic;

namespace Sharpliner.Common;

/// <summary>
/// Represents an interface for validating definitions.
/// </summary>
public interface IDefinitionValidation
{
    /// <summary>
    /// Validates the definitions and returns a collection of validation errors.
    /// </summary>
    /// <returns>A collection of validation errors.</returns>
    IReadOnlyCollection<ValidationError> Validate();
}

/// <summary>
/// Represents the severity of a validation error.
/// </summary>
public enum ValidationSeverity
{
    Off = 0,
    Trace = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
}

/// <summary>
/// Represents a validation error with a severity and message.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets the severity of the validation error.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Gets the message of the validation error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class with the specified severity and message.
    /// </summary>
    /// <param name="severity">The severity of the validation error.</param>
    /// <param name="message">The message of the validation error.</param>
    public ValidationError(ValidationSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
    }
}
