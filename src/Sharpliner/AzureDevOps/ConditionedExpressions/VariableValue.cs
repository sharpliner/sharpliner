using System;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class VariableReference : IRuntimeExpression, ICompileTimeExpression, IMacroExpression, IYamlConvertible
{
    public string VariableName { get; }

    internal VariableReference(string variableName)
    {
        VariableName = variableName;
    }

    public string RuntimeExpression => $"variables['{VariableName}']";

    public string CompileTimeExpression => Condition.ExpressionStart + $"variables.{VariableName}" + Condition.ExpressionEnd;

    public string MacroExpression => $"$({VariableName})";

    public override string ToString() => MacroExpression;

    public static implicit operator string(VariableReference value) => value.ToString();

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        => emitter.Emit(new Scalar(ToString()));
}
