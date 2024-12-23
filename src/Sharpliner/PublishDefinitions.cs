using System;
using System.IO;
using System.Linq;
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
    /// Skip the publishing of YAMLs.
    /// </summary>
    public bool Skip { get; set; }

    /// <summary>
    /// This method finds all pipeline definitions via reflection and publishes them to YAML.
    /// </summary>
    public override bool Execute()
    {
        if (Skip)
        {
            Log.LogMessage(MessageImportance.High, "Skipping the publishing of YAMLs");
            return true;
        }

        // PLEASE READ
        // This method loads and executes the Sharpliner publisher class BUT in the LoadFrom context.
        // We are unable to load the user's assembly into the main binding context because we are running from the NuGet location.
        // The user's assembly is not in the probing path of the Sharpliner NuGet but it has Sharpliner.dll as well.
        // Read more details here: https://github.com/sharpliner/sharpliner/issues/179
        var sharplinerAssemblyPath = Path.Combine(Path.GetDirectoryName(Assembly)!, "Sharpliner.dll");

        var assembly = System.Reflection.Assembly.LoadFrom(sharplinerAssemblyPath);
        if (assembly is null)
        {
            Log.LogError("Failed to load Sharpliner.dll in the target project at {path}", sharplinerAssemblyPath);
            return false;
        }

        var publisherType = assembly.GetTypes().First(t => t.GUID == typeof(SharplinerPublisher).GUID);
        var publisher = Activator.CreateInstance(publisherType, [Log]);

        var publishMethod = publisherType.GetMethod(nameof(SharplinerPublisher.Publish));

        if (publisher is null || publishMethod is null)
        {
            Log.LogError($"Failed to activate the Sharpliner publisher. The is one of the 'should never happen' ones. Please report this");
            return false;
        }

        if (publishMethod!.Invoke(publisher, [Assembly, FailIfChanged]) is not bool result)
        {
            Log.LogError($"Failed to call the Sharpliner publisher. The is one of the 'should never happen' ones. Please report this");
            return false;
        }

        return result;
    }
}
