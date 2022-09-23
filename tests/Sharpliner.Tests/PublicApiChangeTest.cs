﻿using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using PublicApiGenerator;
using Xunit;

namespace Sharpliner.Tests;

public class PublicApiChangeTest
{
    [Fact]
    public void PublicApisHaventChangedTest()
    {
        var api = typeof(ISharplinerDefinition).Assembly.GeneratePublicApi().Trim();
        var exportedApi = GetResourceFile(typeof(PublicApiChangeTest).Assembly, "PublicApiExport.txt").Trim();

        // There can be some unimportant differences in the header
        api = api.Substring(api.IndexOf("namespace "));
        exportedApi = exportedApi.Substring(exportedApi.IndexOf("namespace "));

        api.Should().Be(exportedApi);
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
