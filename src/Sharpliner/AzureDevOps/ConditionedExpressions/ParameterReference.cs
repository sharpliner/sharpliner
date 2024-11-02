using System;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Class that makes it possible to put ${{ parameters['foo'] }} everywhere.
/// </summary>
public class ParameterReference : IRuntimeExpression, ICompileTimeExpression, IYamlConvertible
{
    public string ParameterName { get; }

    internal ParameterReference(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string RuntimeExpression => $"parameters.{ParameterName}";

    public string CompileTimeExpression => Condition.ExpressionStart + RuntimeExpression + Condition.ExpressionEnd;

    public override string ToString() => CompileTimeExpression;

    public static implicit operator string(ParameterReference value) => value.ToString();

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));

    public override bool Equals(object? obj)
    {
        if (obj is ParameterReference other)
        {
            return ParameterName == other.ParameterName;
        }

        return false;
    }

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
