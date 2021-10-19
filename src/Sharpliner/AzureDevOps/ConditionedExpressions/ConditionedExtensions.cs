using System;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedExtensions
    {
        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="conditionedDefinition">Conditioned definition</param>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            string value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="conditionedDefinition">Conditioned definition</param>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            bool value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="conditionedDefinition">Conditioned definition</param>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        public static Conditioned<VariableBase> Variable(
            this Conditioned<VariableBase> conditionedDefinition,
            string name,
            int value)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
            return conditionedDefinition;
        }

        /// <summary>
        /// Defines multiple variables at once.
        /// </summary>
        /// <param name="conditionedDefinition">Conditioned definition</param>
        /// <param name="variables">List of (key, value) pairs</param>
        public static Conditioned<VariableBase> Variables(
            this Conditioned<VariableBase> conditionedDefinition,
            params (string name, object value)[] variables)
        {
            foreach (var variable in variables)
            {
                Conditioned<VariableBase> definition = variable.value switch
                {
                    int number => new Conditioned<VariableBase>(definition: new Variable(variable.name, number)),
                    bool boolean => new Conditioned<VariableBase>(definition: new Variable(variable.name, boolean)),
                    string s => new Conditioned<VariableBase>(definition: new Variable(variable.name, s)),
                    object any => new Conditioned<VariableBase>(definition: new Variable(variable.name, any?.ToString() ?? string.Empty)),
                };

                conditionedDefinition.Definitions.Add(definition);
            }

            return conditionedDefinition;
        }

        /// <summary>
        /// References a variable group.
        /// </summary>
        public static Conditioned<VariableBase> Group(
            this Conditioned<VariableBase> conditionedDefinition,
            string name)
        {
            conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new VariableGroup(name)));
            return conditionedDefinition;
        }

        /// <summary>
        /// Creates a new stage.
        /// </summary>
        public static Conditioned<Stage> Stage(this Conditioned<Stage> condition, Stage stage)
        {
            condition.Definitions.Add(new Conditioned<Stage>(definition: stage));
            return condition;
        }

        /// <summary>
        /// Creates a new step.
        /// </summary>
        public static Conditioned<Step> Step(this Conditioned<Step> condition, Step step)
        {
            condition.Definitions.Add(new Conditioned<Step>(definition: step));
            return condition;
        }

        /// <summary>
        /// Creates a new job.
        /// </summary>
        public static Conditioned<JobBase> Job(this Conditioned<JobBase> condition, JobBase job)
        {
            condition.Definitions.Add(new Conditioned<JobBase>(definition: job));
            return condition;
        }

        /// <summary>
        /// Creates a new deployment job.
        /// </summary>
        public static Conditioned<JobBase> DeploymentJob(this Conditioned<JobBase> condition, JobBase job)
        {
            condition.Definitions.Add(new Conditioned<JobBase>(definition: job));
            return condition;
        }

        /// <summary>
        /// Reference a YAML template.
        /// </summary>
        /// <param name="conditionedDefinition">Conditioned definition</param>
        /// <param name="path">Relative path to the YAML file with the template</param>
        /// <param name="parameters">Values for template parameters</param>
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
