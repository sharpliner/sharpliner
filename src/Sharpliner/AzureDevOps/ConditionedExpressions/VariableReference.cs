using System;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class VariableReference : IRuntimeExpression, IMacroExpression, ICompileTimeExpression, IYamlConvertible
{
    public string VariableName { get; }

    internal VariableReference(string variableName)
    {
        VariableName = variableName;
    }

    public string RuntimeExpression => $"variables['{VariableName}']";

    public string MacroExpression => $"$({VariableName})";

    public string CompileTimeExpression => Condition.ExpressionStart + RuntimeExpression + Condition.ExpressionEnd;

    public override string ToString() => MacroExpression;

    public static implicit operator string(VariableReference value) => value.ToString();

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));

    public static implicit operator Conditioned<int>(VariableReference value) => new ConditionedVariableReference<int>(value);

    public static implicit operator Conditioned<bool>(VariableReference value) => new ConditionedVariableReference<bool>(value);

    public static implicit operator Conditioned<string>(VariableReference value) => new ConditionedVariableReference<string>(value);

    public static implicit operator Conditioned<TimeSpan>(VariableReference value) => new ConditionedVariableReference<TimeSpan>(value);

    public static implicit operator Conditioned<TemplateParameters>(VariableReference value) => new ConditionedVariableReference<TemplateParameters>(value);

    public static implicit operator Conditioned<ConditionedDictionary>(VariableReference value) => new ConditionedVariableReference<ConditionedDictionary>(value);

    public static implicit operator Conditioned<InlineCondition>(VariableReference value) => new ConditionedVariableReference<InlineCondition>(value);

    public static implicit operator Conditioned<VariableBase>(VariableReference value) => new ConditionedVariableReference<VariableBase>(value);

    public static implicit operator Conditioned<Stage>(VariableReference value) => new ConditionedVariableReference<Stage>(value);

    public static implicit operator Conditioned<JobBase>(VariableReference value) => new ConditionedVariableReference<JobBase>(value);

    public static implicit operator Conditioned<Step>(VariableReference value) => new ConditionedVariableReference<Step>(value);

    public static implicit operator Conditioned<Pool>(VariableReference value) => new ConditionedVariableReference<Pool>(value);

    public override bool Equals(object? obj)
    {
        if (obj is VariableReference other)
        {
            return VariableName == other.VariableName;
        }

        return false;
    }

    public override int GetHashCode() => VariableName.GetHashCode();
}

public record ConditionedVariableReference<T> : Conditioned<T>
{
    private readonly VariableReference _variable;

    public ConditionedVariableReference(VariableReference variable) : base()
    {
        _variable = variable;
    }

    public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(_variable.CompileTimeExpression));
}
