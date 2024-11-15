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
    /// <summary>
    /// Used to turn off validation errors.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Validation errors used for debugging and tracing purposes.
    /// </summary>
    Trace = 1,

    /// <summary>
    /// Validation error that should be treated as a information.
    /// </summary>
    Information = 2,

    /// <summary>
    /// Validation error that should be treated as a warning.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Validation error that should be treated as an error.
    /// </summary>
    Error = 4,
}

/// <summary>
/// Represents a validation error with a severity and message.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationError"/> class with the specified severity and message.
/// </remarks>
/// <param name="severity">The severity of the validation error.</param>
/// <param name="message">The message of the validation error.</param>
public class ValidationError(ValidationSeverity severity, string message)
{
    /// <summary>
    /// Gets the severity of the validation error.
    /// </summary>
    public ValidationSeverity Severity { get; } = severity;

    /// <summary>
    /// Gets the message of the validation error.
    /// </summary>
    public string Message { get; } = message;
}
