using System.IO;
using System.Linq;
using System.Reflection;

namespace Sharpliner.Common.Model.Tasks;

/// <summary>
/// Base class for builders that generate pipeline task in a user-friendly way.
/// </summary>
public abstract class TaskBuilderBase
{
    /// <summary>
    /// Helper method to load a resource file from the assembly.
    /// This is used for example when inlining scripts into YAML which are included as embedded resources.
    /// </summary>
    /// <param name="assembly">Assembly the embedded resources is part of</param>
    /// <param name="resourceFileName">Name of the embedded resource file</param>
    /// <returns>Contents of an embedded resource file</returns>
    protected static string GetResourceFile(Assembly assembly, string resourceFileName)
    {
        Stream? stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceFileName}");

        if (stream == null)
        {
            string? resource = assembly.GetManifestResourceNames().FirstOrDefault(res => res.EndsWith(resourceFileName));
            if (resource != null)
            {
                stream = assembly.GetManifestResourceStream(resource);
            }
        }

        using (stream)
        using (var sr = new StreamReader(stream ?? throw new FileNotFoundException($"Couldn't locate resource file '{resourceFileName}'")))
        {
            return sr.ReadToEnd();
        }
    }
}
