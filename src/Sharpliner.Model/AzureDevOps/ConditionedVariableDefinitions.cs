namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// Allows the Variable() and Group() syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedVariableDefinitions
    {
        public static ConditionedDefinition<Variable> Variable(this Condition condition, string name, string value)
            => new(new SingleVariable(name, value), condition.ToString());

        public static ConditionedDefinition<Variable> Variable(this Condition condition, string name, bool value)
            => new(new SingleVariable(name, value), condition.ToString());

        public static ConditionedDefinition<Variable> Variable(this Condition condition, string name, int value)
            => new(new SingleVariable(name, value), condition.ToString());

        public static ConditionedDefinition<Variable> Group(this Condition condition, string name)
            => new(new VariableGroup(name), condition.ToString());

        public static ConditionedDefinition<Variable> Variable(
            this ConditionedDefinition<Variable> condition,
            string name,
            string value)
        {
            condition.Definitions.Add(new SingleVariable(name, value));
            return condition;
        }

        public static ConditionedDefinition<Variable> Variable(
            this ConditionedDefinition<Variable> condition,
            string name,
            bool value)
        {
            condition.Definitions.Add(new SingleVariable(name, value));
            return condition;
        }

        public static ConditionedDefinition<Variable> Variable(
            this ConditionedDefinition<Variable> condition,
            string name,
            int value)
        {
            condition.Definitions.Add(new SingleVariable(name, value));
            return condition;
        }

        public static ConditionedDefinition<Variable> Group(
            this ConditionedDefinition<Variable> condition,
            string name)
        {
            condition.Definitions.Add(new VariableGroup(name));
            return condition;
        }
    }
}
