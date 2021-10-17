using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    /// <summary>
    /// This class is here only to override the Add() which is used for definition.
    /// </summary>
    public class ConditionedList<T> : List<Conditioned<T>>
    {
        // Make sure we can for example assign a string into ConditionedDefinition<string>
        public static implicit operator ConditionedList<T>(List<T> list) => new(list);

        public ConditionedList()
        {
        }

        protected ConditionedList(IEnumerable<T> values)
            : base(values.Select(v => new Conditioned<T>(v)))
        {
        }

        public new void Add(Conditioned<T> item)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the top-level definition
            while (item.Parent is Conditioned<T> parent)
            {
                item = parent;
            }

            SetIsListForAll(item);
            base.Add(item);
        }

        private static void SetIsListForAll(Conditioned? item)
        {
            if (item == null)
            {
                return;
            }

            item.IsList = true;

            if (item is Conditioned<T> temp && temp.Definition is Conditioned temp2)
            {
                SetIsListForAll(temp2);
            }

            foreach (var child in item.Definitions)
            {
                SetIsListForAll(child);
            }
        }
    }
}
