using System.IO;
using System.Linq;
using System.Reflection;

namespace Sharpliner.AzureDevOps.Tasks
{
    public abstract class ScriptTaskBuilder
    {
        protected static string GetResourceFile<TAssembly>(string resourceFileName)
        {
            // TODO: Try GetExecutingAssembly and drop the generics
            Assembly assembly = typeof(TAssembly).Assembly;
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
}
