namespace Sharpliner.Model
{
    /// <summary>
    /// The builder is what let's us start the definition with the "If."
    /// and then forces us to add a condition. The condition then forces us to add
    /// an actual definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionBuilder<T>
    {
        internal ConditionedDefinition<T>? Parent { get; }

        internal ConditionBuilder(ConditionedDefinition<T>? parent = null)
        {
            Parent = parent;
        }

        public Condition<T> Equal(Condition<T> condition)
            => condition;

        public Condition<T> NotEqual(Condition<T> condition)
            => condition;

        public Condition<T> Equal(string expression1, string expression2)
            => new EqualityCondition<T>(expression1, expression2, true)
            {
                Parent = Parent
            };

        public Condition<T> NotEqual(string expression1, string expression2)
            => new EqualityCondition<T>(expression1, expression2, false)
            {
                Parent = Parent
            };

        public Condition<T> And(Condition<T> condition1, Condition<T> condition2)
            => new AndCondition<T>(condition1, condition2)
            {
                Parent = Parent
            };

        public Condition<T> Or(Condition<T> condition1, Condition<T> condition2)
            => new OrCondition<T>(condition1, condition2)
            {
                Parent = Parent
            };

        public Condition And(Condition condition1, Condition condition2)
            => new AndCondition(condition1, condition2)
            {
                Parent = Parent
            };

        public Condition Or(Condition condition1, Condition condition2)
            => new OrCondition(condition1, condition2)
            {
                Parent = Parent
            };
    }
}
