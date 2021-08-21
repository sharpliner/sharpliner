namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedDefinitionsExtensions
    {
        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            string value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            bool value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            int value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Group(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name)
        {
            conditionedDefinition.Definitions.Add(new VariableGroup(name));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<Stage> Stage(this ConditionedDefinition<Stage> condition, Stage stage)
        {
            condition.Definitions.Add(stage);
            return condition;
        }

        public static ConditionedDefinition<Step> Step(this ConditionedDefinition<Step> condition, Step step)
        {
            condition.Definitions.Add(step);
            return condition;
        }

        public static ConditionedDefinition<Job> Job(this ConditionedDefinition<Job> condition, Job job)
        {
            condition.Definitions.Add(job);
            return condition;
        }

        public static ConditionedDefinition<T> Template<T>(
            this ConditionedDefinition<T> conditionedDefinition,
            string path,
            TemplateParameters parameters)
        {
            var template = new Template<T>(path: path, parameters);
            conditionedDefinition.Definitions.Add(template);
            return conditionedDefinition;
        }
    }
}
