namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// Allows the Stage() syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedStageDefinitions
    {
        public static ConditionedDefinition<Stage> Stage(this Condition condition, Stage stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Stage> Stage(
            this ConditionedDefinition<Stage> condition,
            string name,
            string displayName)
        {
            condition.Definitions.Add(new Stage(name, displayName));
            return condition;
        }
    }
}
