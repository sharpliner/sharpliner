namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedDefinitions
    {
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

        public static ConditionedDefinition<T> Template<T>(this ConditionedDefinition<T> conditionedDefinition, string path, TemplateParameters parameters)
        {
            var template = new Template<T>(path: path, parameters);
            conditionedDefinition.Definitions.Add(template);
            return conditionedDefinition;
        }
    }
}
