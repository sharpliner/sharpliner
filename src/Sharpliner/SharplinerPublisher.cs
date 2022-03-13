using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Sharpliner.Common;

namespace Sharpliner;

/// <summary>
/// This is the main entrypoint that finds definitions via reflection and publishes YAMLs.
/// </summary>
public class SharplinerPublisher
{
    private readonly TaskLoggingHelper _logger;

    public SharplinerPublisher(TaskLoggingHelper logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// This method finds all pipeline definitions via reflection and publishes them to YAML.
    /// </summary>
    /// <param name="assemblyPath">Path to assembly with user's Sharpliner definitions</param>
    /// <param name="failIfChanged">
    ///     True to fail the task if there are new changes to be published.
    ///     This is for example used in the ValidateYamlsArePublished build step that checks that YAML changes were checked in.
    /// </param>
    public bool Publish(string assemblyPath, bool failIfChanged)
    {
        var definitionFound = false;
        var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyPath));

        LoadConfiguration(assembly);

        foreach (ISharplinerDefinition definition in FindAllImplementations<ISharplinerDefinition>(assembly))
        {
            definitionFound = true;
            PublishDefinition(definition, failIfChanged);
        }

        foreach ((ISharplinerDefinition definition, Type collection) in FindDefinitionsInCollections(assembly))
        {
            definitionFound = true;
            PublishDefinition(definition, failIfChanged, collection);
        }

        if (!definitionFound)
        {
            _logger.LogMessage(MessageImportance.High, $"No definitions found in {assemblyPath}");
        }

        return true;
    }

    /// <summary>
    /// Publishes given ISharplinerDefinition into a YAML file.
    /// </summary>
    /// <param name="definition">ISharplinerDefinition object</param>
    /// <param name="failIfChanged">
    ///     True to fail the task if there are new changes to be published.
    ///     This is for example used in the ValidateYamlsArePublished build step that checks that YAML changes were checked in.
    /// </param>
    /// <param name="collection">Type of the collection the definition is coming from (if it is)</param>
    private void PublishDefinition(ISharplinerDefinition definition, bool failIfChanged, Type? collection = null)
    {
        var path = definition.GetTargetPath();

        var typeName = collection == null ? definition.GetType().Name : collection.Name + " / " + Path.GetFileName(path);

        _logger.LogMessage(MessageImportance.High, $"{typeName}:");
        _logger.LogMessage(MessageImportance.High, $"  Validating definition..");

        foreach (var validation in definition.Validations)
        {
            var errors = validation.Validate();

            if (errors.Any())
            {
                Log(errors.OrderByDescending(e => e.Severity).First().Severity,
                    $"  Validation of definition {typeName} failed!");
            }

            foreach (var error in errors)
            {
                Log(error.Severity, "    - " + error.Message);
            }
        }

        string? hash = GetFileHash(path);
        definition.Publish();

        if (hash == null)
        {
            if (failIfChanged)
            {
                _logger.LogError($"  This definition hasn't been published yet!");
            }
            else
            {
                _logger.LogMessage(MessageImportance.High, $"  {typeName} created at {path}");
            }
        }
        else
        {
            var newHash = GetFileHash(path);
            if (hash == newHash)
            {
                _logger.LogMessage(MessageImportance.High, $"  No new changes to publish");
            }
            else
            {
                if (failIfChanged)
                {
                    _logger.LogError($"  Changes detected between {typeName} and {path}");
                }
                else
                {
                    _logger.LogMessage(MessageImportance.High, $"  Published new changes to {path}");
                }
            }
        }
    }

    private IEnumerable<(ISharplinerDefinition Definition, Type Collection)> FindDefinitionsInCollections(Assembly assembly) =>
        FindAllImplementations<ISharplinerDefinitionCollection>(assembly)
            .SelectMany(collection => collection.Definitions.Select(definition => (definition, collection.GetType())));

    private List<T> FindAllImplementations<T>(Assembly assembly)
    {
        var pipelines = new List<T>();
        var typeToFind = typeof(T);

        foreach (Type type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeToFind)))
        {
            object? pipelineDefinition = Activator.CreateInstance(type);
            if (pipelineDefinition is null)
            {
                throw new Exception($"Failed to instantiate {type.GetType().FullName}");
            }

            pipelines.Add((T)pipelineDefinition);
        }

        return pipelines;
    }

    private void LoadConfiguration(Assembly assembly)
    {
        var configurations = FindAllImplementations<SharplinerConfiguration>(assembly);

        if (configurations.Count > 1)
        {
            _logger.LogWarning("Detected more than one Sharpliner configurations:"
                + Environment.NewLine + "  -"
                + string.Join(Environment.NewLine + "  -", configurations));
        }

        SharplinerConfiguration configuration = configurations.FirstOrDefault() ?? new DefaultSharplinerConfiguration();
        _logger.LogMessage(MessageImportance.High, "Using {0} for configuration", configuration.GetType().Name);
        configuration.ConfigureInternal();
    }

    private void Log(ValidationSeverity severity, string message)
    {
        switch (severity)
        {
            case ValidationSeverity.Trace:
                _logger.LogMessage(message);
                break;
            case ValidationSeverity.Information:
                _logger.LogMessage(MessageImportance.High, message);
                break;
            case ValidationSeverity.Warning:
                _logger.LogWarning(message);
                break;
            case ValidationSeverity.Error:
                _logger.LogError(message);
                break;
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
