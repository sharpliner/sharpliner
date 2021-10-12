using YamlDotNet.Core.Tokens;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedExtensions
    {
        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            string value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            bool value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            int value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        public static Conditioned<VariableBase> Group(
            this Conditioned<VariableBase> conditionedDefinition,
string name)
{
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new VariableGroup(name)));
            return conditionedDefinition;
        }

        public static Conditioned<Stage> Stage(this Conditioned<Stage> condition, Stage stage)
        {
            condition.Definitions.Add(new Conditioned<Stage>(definition: stage));
            return condition;
        }

        public static Conditioned<Step> Step(this Conditioned<Step> condition, Step step)
        {
            condition.Definitions.Add(new Conditioned<Step>(definition: step));
            return condition;
        }

        public static Conditioned<Job> Job(this Conditioned<Job> condition, Job job)
        {
            condition.Definitions.Add(new Conditioned<Job>(definition: job));
            return condition;
        }

        public static Conditioned<T> Template<T>(
            this Conditioned<T> conditionedDefinition,
            string path,
            TemplateParameters parameters)
        {
            var template = new Template<T>(path: path, parameters);
            conditionedDefinition.Definitions.Add(template);
            return conditionedDefinition;
        }

        internal static Conditioned<T>? GetRoot<T>(this Conditioned<T> conditionedDefinition)
        {
            var parent = conditionedDefinition;
            while (parent?.Parent != null)
            {
                parent = parent.Parent as Conditioned<T>;
            }

            return parent;
        }
    }
}
