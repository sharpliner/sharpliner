namespace Sharpliner.Model
{
    /// <summary>
    /// The builder is what let's us start the definition with the "If."
    /// and then forces us to add a condition. The condition then forces us to add
    /// an actual definition.
    /// </summary>
    public class ConditionBuilder<T>
    {
        internal ConditionedDefinition<T>? Parent { get; }

        internal ConditionBuilder(ConditionedDefinition<T>? parent = null)
        {
            Parent = parent;
        }

        public Condition<T> Equal(Condition<T> condition)
            => Link(condition);

        public Condition<T> NotEqual(Condition<T> condition)
            => Link(condition);

        public Condition<T> Equal(string expression1, string expression2)
            => Link(new EqualityCondition<T>(expression1, expression2, true));

        public Condition<T> NotEqual(string expression1, string expression2)
            => Link(new EqualityCondition<T>(expression1, expression2, false));

        public Condition<T> And(Condition<T> condition1, Condition<T> condition2)
            => Link(new AndCondition<T>(condition1, condition2));

        public Condition<T> Or(Condition<T> condition1, Condition<T> condition2)
            => Link(new OrCondition<T>(condition1, condition2));

        public Condition<T> And(Condition condition1, Condition condition2)
            => Link(new AndCondition<T>(condition1, condition2));

        public Condition<T> Or(Condition condition1, Condition condition2)
            => Link(new OrCondition<T>(condition1, condition2));

        private Condition<T> Link(Condition<T> condition)
        {
            condition.Parent = Parent;
            return condition;
        }
    }
}
