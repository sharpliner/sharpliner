using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Build.Framework;
using Sharpliner.Definition;

namespace Sharpliner
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
            var getPath = type.GetMethod(nameof(PipelineDefinitionBase.GetTargetPath));
            var validate = type.GetMethod(nameof(PipelineDefinitionBase.Validate));
            var publish = type.GetMethod(nameof(PipelineDefinitionBase.Publish));

            if (publish is null || validate is null || getPath is null)
            {
                Log.LogError($"Failed to get pipeline definition metadata for {type.FullName}");
                return;
            }

            if (getPath.Invoke(pipelineDefinition, null) is not string path)
            {
                Log.LogError($"Failed to get target path for {type.Name} ");
                return;
            }

            Log.LogMessage(MessageImportance.High, $"Validating pipeline {type.Name}..");

            try
            {
                validate.Invoke(pipelineDefinition, null);
            }
            catch (Exception e)
            {
                Log.LogMessage(MessageImportance.High, $"Validation of pipeline {type.Name} failed: {e.Message}");
                return;
            }

            Log.LogMessage(MessageImportance.High, $"Publishing pipeline {type.Name} to {path}..");

            string? hash = GetFileHash(path);

            // Publish pipeline
            publish.Invoke(pipelineDefinition, null);

            if (hash == null)
            {
                Log.LogMessage(MessageImportance.High, $"YAML for {type.Name} created at {path}");
            }
            else
            {
                var newHash = GetFileHash(path);
                if (hash == newHash)
                {
                    Log.LogMessage(MessageImportance.High, $"No new changes to publish for {type.Name}");
                }
                else
                {
                    Log.LogMessage(MessageImportance.High, $"Published new changes for {type.Name}");
                }
            }
        }

        private static string? GetFileHash(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            return System.Text.Encoding.UTF8.GetString(md5.ComputeHash(stream));
        }
    }
}
