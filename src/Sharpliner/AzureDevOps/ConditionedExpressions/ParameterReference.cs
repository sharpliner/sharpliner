using System;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Class that makes it possible to put <c>${{ parameters['foo'] }}</c> everywhere.
/// </summary>
public class ParameterReference : IRuntimeExpression, ICompileTimeExpression, IYamlConvertible
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string ParameterName { get; }

    internal ParameterReference(string parameterName)
    {
        ParameterName = parameterName;
    }

    /// <inheritdoc/>
    public string RuntimeExpression => $"parameters.{ParameterName}";

    /// <inheritdoc/>
    public string CompileTimeExpression => Condition.ExpressionStart + RuntimeExpression + Condition.ExpressionEnd;

    /// <summary>
    /// Returns string representation of the variable reference as a compile-time expression.
    /// </summary>
    /// <returns>The compile-time expression.</returns>
    public override string ToString() => CompileTimeExpression;

    /// <summary>
    /// Implicitly converts the <see cref="ParameterReference"/> to a string by returning the compile-time expression.
    /// </summary>
    /// <param name="value">The parameter reference.</param>
    public static implicit operator string(ParameterReference value) => value.ToString();

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is ParameterReference other)
        {
            return ParameterName == other.ParameterName;
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => ParameterName.GetHashCode();
}

internal record ConditionedParameterReference<T> : Conditioned<T>
{
    private readonly ParameterReference _parameter;

    public ConditionedParameterReference(ParameterReference parameter) : base()
    {
        _parameter = parameter;
    }

    internal override void WriteInternal(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(_parameter.CompileTimeExpression));
}
