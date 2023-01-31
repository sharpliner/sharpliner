using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PublicApiGenerator;
using Xunit;

namespace Sharpliner.Tests;

public class PublicApiChangeTest
{
    [Fact]
    public void PublicApisHaventChangedTest()
    {
        var actualApi = typeof(ISharplinerDefinition).Assembly.GeneratePublicApi().Trim();
        var expectedApi = GetResourceFile(typeof(PublicApiChangeTest).Assembly, "PublicApiExport.txt").Trim();

        // There can be some unimportant differences in the header
        actualApi = actualApi.Substring(actualApi.IndexOf("namespace ")).Trim();
        expectedApi = expectedApi.Substring(expectedApi.IndexOf("namespace ")).Trim();

        if (actualApi != expectedApi)
        {
            throw new Exception("Detected a change in the public API of the library. If the change is intentional, please update the PublicApiExport.txt file."
                + Environment.NewLine
                + "Expected content:"
                + Environment.NewLine
                + actualApi);
        }
    }

    private static string GetResourceFile(Assembly assembly, string resourceFileName)
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
