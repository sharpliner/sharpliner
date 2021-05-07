using System.Collections.Generic;
using System;
using Microsoft.Build.Framework;
using System.Linq;
using Sharpliner.AzureDevOps;

namespace Sharpliner.Tools
{
    public class PublishPipelines : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Assembly that will be scaned for pipeline definitions.
        /// </summary>
        [Required]
        public string? Assembly { get; set; }

        public override bool Execute()
        {
            var definitions = FindPipelines<AzureDevOpsPipelineDefinition>();

            foreach (var definition in definitions)
            {
                Log.LogMessage(MessageImportance.High, "Found " + definition.TargetFile);
            }

            return true;
        }

        private IEnumerable<T> FindPipelines<T>() where T : class
        {
            var assembly = System.Reflection.Assembly.LoadFile(Assembly ?? throw new Exception("Failed to read current assembly name"));

            var objects = new List<T>();
            foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
            {
                if (Activator.CreateInstance(type) is not T obj)
                {
                    throw new Exception($"Failed to instantiate {type.GetType().FullName}");
                }

                objects.Add(obj);
            }

            objects.Sort();
            return objects;
        } 
    }
}
