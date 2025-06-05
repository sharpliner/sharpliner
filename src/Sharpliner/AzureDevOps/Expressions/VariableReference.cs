using System;
using Sharpliner.AzureDevOps.Expressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// Class that makes it possible to put <c>${{ variables['foo'] }}</c> everywhere.
/// </summary>
public class VariableReference : IRuntimeExpression, IMacroExpression, ICompileTimeExpression, IYamlConvertible
{
    /// <summary>
    /// Gets the name of the variable.
    /// </summary>
    public string VariableName { get; }

    internal VariableReference(string variableName)
    {
        VariableName = variableName;
    }

    /// <inheritdoc/>
    public virtual string RuntimeExpression => $"variables['{VariableName}']";

    /// <inheritdoc/>
    public virtual string MacroExpression => $"$({VariableName})";

    /// <inheritdoc/>
    public virtual string CompileTimeExpression => Condition.ExpressionStart + RuntimeExpression + Condition.ExpressionEnd;

    /// <summary>
    /// Returns string representation of the variable reference as a macro expression.
    /// </summary>
    /// <returns>The macro expression.</returns>
    public override string ToString() => MacroExpression;

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a string by returning the macro expression.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator string(VariableReference value) => value.ToString();

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="int"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<int>(VariableReference value) => new VariableReferenceExpression<int>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<bool>(VariableReference value) => new VariableReferenceExpression<bool>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="string"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<string>(VariableReference value) => new VariableReferenceExpression<string>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<TimeSpan>(VariableReference value) => new VariableReferenceExpression<TimeSpan>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="TemplateParameters"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<TemplateParameters>(VariableReference value) => new VariableReferenceExpression<TemplateParameters>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="DictionaryExpression"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<DictionaryExpression>(VariableReference value) => new VariableReferenceExpression<DictionaryExpression>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="InlineCondition"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<InlineCondition>(VariableReference value) => new VariableReferenceExpression<InlineCondition>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="VariableBase"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<VariableBase>(VariableReference value) => new VariableReferenceExpression<VariableBase>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="Stage"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<Stage>(VariableReference value) => new VariableReferenceExpression<Stage>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="JobBase"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<JobBase>(VariableReference value) => new VariableReferenceExpression<JobBase>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="Step"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<Step>(VariableReference value) => new VariableReferenceExpression<Step>(value);

    /// <summary>
    /// Implicitly converts the <see cref="VariableReference"/> to a <see cref="AdoExpression{T}"/> of type <see cref="Pool"/>.
    /// </summary>
    /// <param name="value">The variable reference.</param>
    public static implicit operator AdoExpression<Pool>(VariableReference value) => new VariableReferenceExpression<Pool>(value);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is VariableReference other)
        {
            return VariableName == other.VariableName;
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => VariableName.GetHashCode();
}

/// <summary>
/// Represents a conditioned variable reference.
/// </summary>
/// <typeparam name="T">The type of the variable reference.</typeparam>
public record VariableReferenceExpression<T> : AdoExpression<T>
{
    private readonly VariableReference _variable;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableReferenceExpression{T}"/> class with the given <see cref="VariableReference"/>.
    /// </summary>
    /// <param name="variable">The variable reference.</param>
    public VariableReferenceExpression(VariableReference variable) : base()
    {
        _variable = variable;
    }

    internal override void WriteInternal(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(_variable.MacroExpression));
}
