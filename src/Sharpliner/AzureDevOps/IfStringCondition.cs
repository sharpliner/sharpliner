using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Expressions.Arguments;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// <para>
/// Base class for string conditions. to be used with <see cref="IfExpression"/> and <see cref="IfArrayExpression"/>
/// </para>
/// Can be used with <see cref="IfConditionBuilder.Equal(IfExpression, IfExpression)"/> and <see cref="IfConditionBuilder.NotEqual(IfExpression, IfExpression)"/> and more.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
/// </remarks>
/// <param name="keyword">The function keyword.</param>
/// <param name="one">The first expression.</param>
/// <param name="two">The second expression.</param>
public abstract class IfStringCondition(string keyword, IfExpression one, IfExpression two)
    : IfCondition
{
    private readonly string _keyword = keyword;
    private readonly string _one = Serialize(one);
    private readonly string _two = Serialize(two);

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first array expression.</param>
    /// <param name="two">The second expression.</param>
    protected IfStringCondition(string keyword, IfArrayExpression one, IfExpression two)
        : this(keyword, Serialize(one), two)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second array expression.</param>
    protected IfStringCondition(string keyword, IfExpression one, IfArrayExpression two)
        : this(keyword, one, Serialize(two))
    {
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}

/// <summary>
/// Base class for string conditions. This type is used for chaining conditions.
/// </summary>
/// <typeparam name="T">The type of the parent condition.</typeparam>
public abstract class IfStringCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition{T}"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first array expression.</param>
    /// <param name="two">The second expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected IfStringCondition(string keyword, IfArrayExpression one, IfExpression two, AdoExpression<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition{T}"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second array expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected IfStringCondition(string keyword, IfExpression one, IfArrayExpression two, AdoExpression<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition{T}"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second expression.</param>
    /// <param name="parent">The parent condition.</param>
    protected IfStringCondition(string keyword, IfExpression one, IfExpression two, AdoExpression<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}
