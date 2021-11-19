using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Build.Framework;

namespace Sharpliner;

/// <summary>
/// This is an MSBuild task that is run in user projects to publish YAMLs after build.
/// </summary>
public class PublishDefinitions : Microsoft.Build.Utilities.Task
{
    /// <summary>
    /// Assembly that will be scaned for pipeline definitions.
    /// </summary>
    [Required]
    public string? Assembly { get; set; }

    /// <summary>
    /// You can make the task fail in case it finds a YAML whose definition changed.
    /// This is for example used in the ValidateYamlsArePublished build step that checks that YAML changes were checked in.
    /// </summary>
    public bool FailIfChanged { get; set; }

    /// <summary>
    /// Changes how we log errors in case we are running this task as part of an Azure DevOps pipeline.
    /// </summary>
    public bool IsAzureDevOpsBuild { get; set; }

    /// <summary>
    /// This method finds all pipeline definitions via reflection and publishes them to YAML.
    /// </summary>
    public override bool Execute()
    {
        var definitionFound = false;

        foreach (var definition in FindAllImplementations<ISharplinerDefinition>(isInterface: true))
        {
            definitionFound = true;
            PublishDefinition(definition);
        }

        foreach (var d in FindDefinitionsInCollections())
        {
            definitionFound = true;
            PublishDefinition(d.Definition, d.Collection);
        }

        return definitionFound;
    }

    /// <summary>
    /// Publishes given ISharplinerDefinition into a YAML file.
    /// </summary>
    /// <param name="definition">ISharplinerDefinition object</param>
    /// <param name="collection">Type of the collection the definition is coming from (if it is)</param>
    private void PublishDefinition(object definition, Type? collection = null)
    {
        // I am unable to just cat to IDefinition for some reason (they come from the same code, but maybe different .dll files or something)
        Type type = definition.GetType();
        Type iface = type.GetInterfaces().First(i => i.GUID == typeof(ISharplinerDefinition).GUID);
        var getPath = iface.GetMethod(nameof(ISharplinerDefinition.GetTargetPath));
        var validate = iface.GetMethod(nameof(ISharplinerDefinition.Validate));
        var publish = iface.GetMethod(nameof(ISharplinerDefinition.Publish));

        if (publish is null || validate is null || getPath is null)
        {
            LogError($"Failed to get definition metadata for {collection?.FullName ?? type.FullName}");
            return;
        }

        if (getPath.Invoke(definition, null) is not string path)
        {
            LogError($"Failed to get target path for {collection?.Name ?? type.Name} ");
            return;
        }

        var typeName = collection == null ? type.Name : collection.Name + " / " + Path.GetFileName(path);

        Log.LogMessage(MessageImportance.High, $"{typeName}:");
        Log.LogMessage(MessageImportance.High, $"  Validating definition..");

        try
        {
            validate.Invoke(definition, null);
        }
        catch (TargetInvocationException e)
        {
            var errorMessage = string.Format("Validation of definition {0} failed: {1}{2}{3}",
                typeName,
                e.InnerException?.Message ?? e.ToString(),
                Environment.NewLine,
                "To see exception details, build with more verbosity (dotnet build -v:n)");

            LogError(errorMessage);
            Log.LogMessage(MessageImportance.Normal, "Validation of definition {0} failed: {1}", typeName, e.InnerException);
            return;
        }

        string? hash = GetFileHash(path);

        // Publish pipeline
        publish.Invoke(definition, null);

        if (hash == null)
        {
            if (FailIfChanged)
            {
                LogError($"  This definition hasn't been published yet!");
            }
            else
            {
                Log.LogMessage(MessageImportance.High, $"  {typeName} created at {path}");
            }
        }
        else
        {
            var newHash = GetFileHash(path);
            if (hash == newHash)
            {
                Log.LogMessage(MessageImportance.High, $"  No new changes to publish");
            }
            else
            {
                if (FailIfChanged)
                {
                    LogError($"  Changes detected between {typeName} and {path}!");
                }
                else
                {
                    Log.LogMessage(MessageImportance.High, $"  Published new changes to {path}");
                }
            }
        }
    }

    private IEnumerable<(object Definition, Type Collection, int Number)> FindDefinitionsInCollections()
    {
        var collections = FindAllImplementations<ISharplinerDefinitionCollection>(isInterface: true);

        foreach (var collection in collections)
        {
            var type = collection.GetType();
            Type iface = type.GetInterfaces().First(i => i.GUID == typeof(ISharplinerDefinitionCollection).GUID);

            var definitionsProperty = iface.GetProperty(nameof(ISharplinerDefinitionCollection.Definitions))?.GetValue(collection, null);
            if (definitionsProperty is not IEnumerable definitions)
            {
                LogError($"Failed to get definitions from collection {type.FullName}");
                continue;
            }

            int i = 1;
            foreach (var definition in definitions)
            {
                yield return (definition, type, i++);
            }
        }
    }

    private List<object> FindAllImplementations<T>(bool isInterface)
    {
        var assembly = LoadAssembly(Assembly ?? throw new ArgumentNullException(nameof(Assembly), "Assembly parameter not set"));

        var pipelines = new List<object>();
        var pipelineBaseType = typeof(T);

        foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            if (isInterface)
            {
                // I am unable to just do "is T" because the types from the assembly cannot be cast for some reasone
                // (they come from the same code, but maybe different .dll files or something)..
                // I tried to make sure there is only one Sharpliner.dll but still couldn't get it to work so we have to treat them as anonymous
                var interfaces = type.GetInterfaces();
                if (!interfaces.Any(iface => iface.FullName == typeof(T).FullName && iface.GUID == typeof(T).GUID))
                {
                    continue;
                }
            }
            else
            {
                bool isChild = false;
                var baseType = type.BaseType;

                // I am unable to cast this to PipelineDefinitionBase and just do t.IsSubClass or t.IsAssignableTo because the types don't seem
                // to be the same even when they are (they come from the same code, but maybe different .dll files)..
                // I tried to make sure there is only one Sharpliner.dll but still couldn't get it to work so we have to parse invoke Publish via reflection
                while (!isChild && baseType is not null)
                {
                    isChild |= baseType.FullName == typeof(T).FullName && baseType.GUID == typeof(T).GUID;
                    baseType = baseType.BaseType;
                }

                if (!isChild)
                {
                    continue;
                }
            }

            object? pipelineDefinition = Activator.CreateInstance(type);
            if (pipelineDefinition is null)
            {
                throw new Exception($"Failed to instantiate {type.GetType().FullName}");
            }

            pipelines.Add(pipelineDefinition);
        }

        return pipelines;
    }

    /// <summary>
    /// Loads the assembly and all of its dependencies (such as YamlDotNet).
    /// </summary>
    private Assembly LoadAssembly(string assemblyPath)
    {
        // Preload dependencies needed for things to work
        var assemblies = new[] { "YamlDotNet.dll", "Sharpliner.dll" }
            .Select(assemblyName => Path.Combine(Path.GetDirectoryName(assemblyPath) ?? throw new Exception($"Failed to find directory of {assemblyPath}"), assemblyName))
            .Select(Path.GetFullPath)
            .Select(path => System.Reflection.Assembly.LoadFile(path) ?? throw new Exception($"Failed to find a Sharpliner dependency at {path}. Make sure the bin/ directory of your project contains this library."))
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
        return System.Reflection.Assembly.LoadFile(Path.GetFullPath(assemblyPath));
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

    private void LogError(string message, params string[] objects)
    {
        if (objects.Any())
        {
            message = string.Format(message, objects);
        }

        Log.LogError(IsAzureDevOpsBuild ? $"##[error]{message}" : message);
    }
}
