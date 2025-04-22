using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// <para>
/// Represents an inline condition that can be used in Azure DevOps YAML.
/// </para>
/// For example, <see cref="AzureDevOpsDefinition.Equal(InlineExpression, InlineExpression)"/> and <see cref="AzureDevOpsDefinition.NotEqual(InlineExpression, InlineExpression)"/>.
/// </summary>
public abstract class InlineCondition : Condition
{
    internal static string Serialize(InlineArrayExpression arrayValue)
    {
        return InlineStringConditionHelper.Serialize(arrayValue);
    }

    internal static string Serialize(InlineExpression stringOrVariableOrParameter)
    {
        return InlineStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    /// <summary>
    /// Returns InlineCondition "succeeded()".
    /// </summary>
    public static string Succeeded => new InlineCustomCondition("succeeded()");
}

/// <summary>
/// <para>
/// Represents an inline condition that can be used in Azure DevOps YAML. This type is used for chaining conditions.
/// </para>
/// For example, <see cref="AzureDevOpsDefinition.Equal{T}(InlineExpression, InlineExpression)"/> and <see cref="AzureDevOpsDefinition.NotEqual{T}(InlineExpression, InlineExpression)"/>.
/// </summary>
/// <typeparam name="T">The type of the parent condition.</typeparam>
public abstract class InlineCondition<T> : InlineCondition
{
    /// <summary>
    /// Creates a new instance of <see cref="InlineCondition{T}"/> with the specified parent condition.
    /// </summary>
    /// <param name="parent">The parent condition.</param>
    protected InlineCondition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }
}
