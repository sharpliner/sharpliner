using System.Collections.Generic;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// A dictionary that can contain <see cref="Conditioned"/> items.
/// </summary>
public class ConditionedDictionary : Dictionary<string, object>
{
    /// <summary>
    /// Adds a new item to the dictionary.
    /// If the item is a <see cref="Conditioned"/> item, it will be marked as a single item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The value.</param>
    public new void Add(string key, object item) => base.Add(key, GetRootConditioned(item));

    public new object this[string key]
    {
        get => base[key];
        set => base[key] = GetRootConditioned(value);
    }

    private static object GetRootConditioned(object item)
    {
        if (item is Conditioned conditioned)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the root item
            while (conditioned.Parent is Conditioned parent)
            {
                conditioned = parent;
            }

            return conditioned;
        }

        return item;
    }
}
