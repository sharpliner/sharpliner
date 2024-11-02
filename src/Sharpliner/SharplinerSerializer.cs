﻿using System;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner;

internal static class SharplinerSerializer
{
    public static ISerializer Serializer { get; } = InitializeSerializer();

    public static string Serialize(object data)
    {
        var yaml = Serializer.Serialize(data);
        return SharplinerConfiguration.Current.Serialization.PrettifyYaml ? Prettify(yaml) : yaml;
    }

    private static ISerializer InitializeSerializer()
    {
        var serializerBuilder = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlStringEnumConverter())
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .WithEventEmitter(nextEmitter => new MultilineStringEmitter(nextEmitter));

        return serializerBuilder.Build();
    }

    public static string Prettify(string yaml)
    {
        // Add empty new lines to make text more readable
        var newLineReplace = Environment.NewLine + "$1";
        yaml = s_sectionStartRegex.Replace(yaml, newLineReplace);
        yaml = s_mainItemStartRegex.Replace(yaml, newLineReplace);
        yaml = s_conditionedBlockStartRegex.Replace(yaml, newLineReplace);
        yaml = s_doubleNewLineStartRegex.Replace(yaml, ":" + Environment.NewLine);
        return yaml;
    }

    private static readonly Regex s_sectionStartRegex = new("((\r?\n)[a-zA-Z]+:)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_mainItemStartRegex = new("((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_conditionedBlockStartRegex = new("((\r?\n) {0,8}- ?\\${{ ?(if|else|each|parameters|variables)[^\n]+\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_doubleNewLineStartRegex = new("(:\r?\n\r?\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
