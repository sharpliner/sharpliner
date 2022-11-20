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

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));

    public static implicit operator Conditioned<VariableBase>(ParameterReference value) => new ConditionedParameterReference<VariableBase>(value);

    public static implicit operator Conditioned<Stage>(ParameterReference value) => new ConditionedParameterReference<Stage>(value);

    public static implicit operator Conditioned<JobBase>(ParameterReference value) => new ConditionedParameterReference<JobBase>(value);

    public static implicit operator Conditioned<Step>(ParameterReference value) => new ConditionedParameterReference<Step>(value);

    public static implicit operator Conditioned<Pool>(ParameterReference value) => new ConditionedParameterReference<Pool>(value);
}

public record ConditionedParameterReference<T> : Conditioned<T>
{
    private readonly ParameterReference _parameter;

    public ConditionedParameterReference(ParameterReference parameter) : base()
    {
        _parameter = parameter;
    }

    public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(_parameter.CompileTimeExpression));
}
