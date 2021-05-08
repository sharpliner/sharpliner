using System;
using Microsoft.Build.Framework;
using System.Linq;
using Sharpliner.AzureDevOps;
using Sharpliner.Definition;

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
            // TODO: We can just search for PipelineDefinitionBase instead and publish all pipeline types
            PublishAllPipelines<AzureDevOpsPipelineDefinition>(Assembly ?? throw new Exception("Failed to read current assembly name"));
            return true;
        }

        private bool PublishAllPipelines<T>(string assemblyPath) where T : PipelineDefinitionBase
        {
            var pipelineFound = false;
            var assembly = System.Reflection.Assembly.LoadFile(assemblyPath);
            var pipelineBaseType = typeof(T);

            Log.LogMessage(MessageImportance.High, $"Searching for {typeof(T).FullName}");

            // TODO: I am unable to cast this to PipelineDefinitionBase and just do t.IsSubClass or t.IsAssignableTo because
            // the types don't seem to be the same even when they are..
            // I tried to make sure there is only one Sharpliner.dll but still couldn't get it to work so we have to parse members dynamically
            foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                bool isPipelineDefinition = false;
                var baseType = type.BaseType;
                while (baseType is not null)
                {
                    isPipelineDefinition |= baseType.FullName == typeof(T).FullName && baseType.GUID == typeof(T).GUID;
                    baseType = baseType.BaseType;
                }

                if (!isPipelineDefinition)
                {
                    continue;
                }

                object? pipelineDefinition = Activator.CreateInstance(type);
                if (pipelineDefinition is null)
                {
                    throw new Exception($"Failed to instantiate {type.GetType().FullName}");
                }

                pipelineFound = true;
                PublishPipeline(pipelineDefinition);
            }

            return pipelineFound;
        }

        private void PublishPipeline(object pipelineDefinition)
        {
            Type type = pipelineDefinition.GetType();
            var method = type.GetMethod(nameof(PipelineDefinitionBase.Publish));

            if (method is null)
            {
                Log.LogError($"Failed to get pipeline definition metadata for {type.FullName}");
                return;
            }

            Log.LogMessage(MessageImportance.High, $"Publishing pipeline {type.Name}");
            method.Invoke(pipelineDefinition, null);
        }
    }
}
