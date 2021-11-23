using System.Collections.Generic;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class ConditionedDictionary : Dictionary<string, object>
{
    public new void Add(string key, object item)
    {
        if (item is Conditioned conditioned)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the root item
            while (conditioned.Parent is Conditioned parent)
            {
                conditioned = parent;
            }

            conditioned.SetIsList(false);
            item = conditioned;
        }

        base.Add(key, item);
    }
}
