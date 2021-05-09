using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
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

        public override bool Execute() => PublishAllPipelines<PipelineDefinitionBase>();

        private bool PublishAllPipelines<T>() where T : PipelineDefinitionBase
        {
            var assembly = LoadAssembly(Assembly ?? throw new ArgumentNullException(nameof(Assembly), "Assembly parameter not set"));

            var pipelineFound = false;
            var pipelineBaseType = typeof(T);
            foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                bool isPipelineDefinition = false;
                var baseType = type.BaseType;

                // TODO: I am unable to cast this to PipelineDefinitionBase and just do t.IsSubClass or t.IsAssignableTo because the types don't seem
                // to be the same even when they are (they come from the same code, but maybe different .dll files)..
                // I tried to make sure there is only one Sharpliner.dll but still couldn't get it to work so we have to parse invoke Publish via reflection
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

        /// <summary>
        /// Loads the assembly and all of its dependencies (such as YamlDotNet).
        /// </summary>
        private Assembly LoadAssembly(string assemblyPath)
        {
            // Preload dependencies needed for things to work
            var assemblies = new[] { "YamlDotNet.dll", "Sharpliner.dll" }
                .Select(assemblyName => Path.Combine(Path.GetDirectoryName(assemblyPath) ?? throw new Exception($"Failed to find directory of {assemblyPath}"), assemblyName))
                .Select(path => System.Reflection.Assembly.LoadFile(path) ?? throw new Exception($"Failed to find a Sharpliner dependency at {path}. Make sure your bin/ contains this library."))
                .Where(a => a is not null)
                .ToDictionary(a => a.FullName!);

            Assembly ResolveAssembly(object? sender, ResolveEventArgs e)
            {
                if (!assemblies.TryGetValue(e.Name, out var res))
                {
                    throw new Exception("Failed to find Sharpliner dependency " + e.Name);
                }

                return res;
            }

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            // Load the final assembly where pipeline is defined
            return System.Reflection.Assembly.LoadFile(assemblyPath);
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
