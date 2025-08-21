using System.Collections.Generic;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// A dictionary that can contain <see cref="AdoExpression"/> items.
/// </summary>
public class DictionaryExpression : Dictionary<string, object>
{
    public DictionaryExpression()
    {
    }

    public DictionaryExpression(Dictionary<string, object> other)
        : base(other)
    {   
    }

    /// <summary>
    /// Adds a new item to the dictionary.
    /// If the item is a <see cref="AdoExpression"/> item, it will be marked as a single item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The value.</param>
    public new void Add(string key, object item) => base.Add(key, GetRootExpression(item));

    /// <summary>
    /// Gets or sets the item with the specified key.
    /// If the item is a <see cref="AdoExpression"/> item, it will be marked as a single item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The value</returns>
    public new object this[string key]
    {
        get => base[key];
        set => base[key] = GetRootExpression(value);
    }

    private static object GetRootExpression(object item)
    {
        if (item is AdoExpression conditioned)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the root item
            while (conditioned.Parent is AdoExpression parent)
            {
                conditioned = parent;
            }

            return conditioned;
        }

        return item;
    }
}
