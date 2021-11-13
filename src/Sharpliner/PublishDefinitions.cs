using System;
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

    public override bool Execute() => PublishAllDefinitions();

    /// <summary>
    /// This method finds all pipeline definitions via reflection and publishes them to YAML.
    /// </summary>
    private bool PublishAllDefinitions()
    {
        var pipelines = FindAllImplementations<IDefinition>();

        foreach (var pipeline in pipelines)
        {
            PublishDefinition(pipeline);
        }

        return pipelines.Any();
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

    private void PublishDefinition(object definition)
    {
        // I am unable to just cat to IDefinition for some reason (they come from the same code, but maybe different .dll files or something)
        Type type = definition.GetType();
        Type iface = type.GetInterfaces().First(i => i.GUID == typeof(IDefinition).GUID);
        var getPath = iface.GetMethod(nameof(IDefinition.GetTargetPath));
        var serialize = type.GetMethod(nameof(IDefinition.Serialize));
        var validate = type.GetMethod(nameof(IDefinition.Validate));
        var header = type.GetProperty(nameof(IDefinition.Header));

        if (serialize is null || validate is null || getPath is null || header is null)
        {
            Log.LogError($"Failed to get pipeline definition metadata for {type.FullName}");
            return;
        }

        if (getPath.Invoke(definition, null) is not string path)
        {
            Log.LogError($"Failed to get target path for {type.Name} ");
            return;
        }

        Log.LogMessage(MessageImportance.High, $"{type.Name}:");
        Log.LogMessage(MessageImportance.High, $"  Validating pipeline..");

        try
        {
            validate.Invoke(definition, null);
        }
        catch (TargetInvocationException e)
        {
            Log.LogError("Validation of pipeline {0} failed: {1}{2}{3}",
                type.Name,
                e.InnerException?.Message ?? e.ToString(),
                Environment.NewLine,
                e);// TODO "To see exception details, build with more verbosity (dotnet build -v:n)");
            Log.LogMessage(MessageImportance.Normal, "Validation of pipeline {0} failed: {1}", type.Name, e.InnerException);
            return;
        }

        string? content = serialize.Invoke(definition, null) as string;
        string? fileHeader = header.GetValue(definition, null) as string;
        string? hash = GetFileHash(path);

        // Publish pipeline
        PublishDefinition(path, fileHeader!, content!);

        if (hash == null)
        {
            if (FailIfChanged)
            {
                Log.LogError($"  This pipeline hasn't been published yet!");
            }
            else
            {
                Log.LogMessage(MessageImportance.High, $"  {type.Name} created at {path}");
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
                    Log.LogError($"  Changes detected between {type.Name} and {path}!");
                }
                else
                {
                    Log.LogMessage(MessageImportance.High, $"  Published new changes to {path}");
                }
            }
        }
    }

    private void PublishDefinition(string targetPath, string fileHeader, string fileContents)
    {
        if (fileHeader?.Length > 0)
        {
            const string hash = "### ";
            fileContents = hash + string.Join(Environment.NewLine + hash, fileHeader) + Environment.NewLine + Environment.NewLine + fileContents;
            fileContents = fileContents.Replace(" \r\n", "\r\n").Replace(" \n", "\n"); // Remove trailing spaces from the default template
        }

        var targetDirectory = Path.GetDirectoryName(targetPath)!;
        if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        File.WriteAllText(targetPath, fileContents);
    }

    private List<object> FindAllImplementations<T>()
    {
        var assembly = LoadAssembly(Assembly ?? throw new ArgumentNullException(nameof(Assembly), "Assembly parameter not set"));

        var pipelines = new List<object>();
        var pipelineBaseType = typeof(T);

        foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            // I am unable to just do "is IDefinition) because the types from the assembly cannot be cast for some reasone
            // (they come from the same code, but maybe different .dll files or something)..
            // I tried to make sure there is only one Sharpliner.dll but still couldn't get it to work so we have to treat them as anonymous
            var interfaces = type.GetInterfaces();
            if (!interfaces.Any(iface => iface.FullName == typeof(T).FullName && iface.GUID == typeof(T).GUID))
            {
                continue;
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
