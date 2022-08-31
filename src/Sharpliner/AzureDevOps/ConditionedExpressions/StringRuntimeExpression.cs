using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class StringRuntimeExpression : IRuntimeExpression
{
    internal StringRuntimeExpression(string expression)
    {
        RuntimeExpression = expression;
    }

    public string RuntimeExpression { get; }

    public override string ToString() => RuntimeExpression;

    public static implicit operator StringRuntimeExpression(string value) => new(value);
}
