using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// <para>
/// Base class for string conditions. to be used with <see cref="InlineExpression"/> and <see cref="InlineArrayExpression"/>
/// </para>
/// Can be used with <see cref="AzureDevOpsDefinition.Equal(InlineExpression, InlineExpression)"/> and <see cref="AzureDevOpsDefinition.NotEqual(InlineExpression, InlineExpression)"/> and more.
/// </summary>
public abstract class InlineStringCondition : InlineCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first array expression.</param>
    /// <param name="two">The second expression.</param>
    protected InlineStringCondition(string keyword, InlineArrayExpression one, InlineExpression two)
        : this(keyword, Serialize(one), two)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second array expression.</param>
    protected InlineStringCondition(string keyword, InlineExpression one, InlineArrayExpression two)
        : this(keyword, one, Serialize(two))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second expression.</param>
    protected InlineStringCondition(string keyword, InlineExpression one, InlineExpression two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}

/// <summary>
/// <para>
/// Base class for string conditions. to be used with <see cref="InlineExpression"/> and <see cref="InlineArrayExpression"/>.
/// This type is used for chaining conditions.
/// </para>
/// Can be used with <see cref="AzureDevOpsDefinition.Equal{T}(InlineExpression, InlineExpression)"/> and <see cref="AzureDevOpsDefinition.NotEqual{T}(InlineExpression, InlineExpression)"/> and more.
/// </summary>
/// <typeparam name="T">The type of the parent condition.</typeparam>
public abstract class InlineStringCondition<T> : InlineCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first array expression.</param>
    /// <param name="two">The second expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected InlineStringCondition(string keyword, InlineArrayExpression one, InlineExpression two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second array expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected InlineStringCondition(string keyword, InlineExpression one, InlineArrayExpression two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="InlineStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected InlineStringCondition(string keyword, InlineExpression one, InlineExpression two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}
