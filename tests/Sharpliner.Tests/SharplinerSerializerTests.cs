using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests;

public class SharplinerSerializerTests
{
    [Fact]
    public Task SharedObjectReference_Test()
    {
        // This test ensures that when the same object instance is referenced multiple times in a dictionary,
        // each reference is serialized with its full content rather than using YAML anchors/aliases.
        // Azure DevOps Pipelines don't support YAML anchors/aliases, so we need to disable them.
        
        var sharedValue = new Dictionary<string, object> { { "SKU", "D16" } };

        var dict = new DictionaryExpression
        {
            { "LinuxHostVersion", sharedValue },
            { "WindowsHostVersion", sharedValue }
        };

        return Verify(SharplinerSerializer.Serialize(dict));
    }

    [Fact]
    public void SharedObjectReference_NoAnchorsOrAliases()
    {
        // Verify that the serialized output doesn't contain YAML anchors or aliases
        var sharedValue = new Dictionary<string, object> { { "SKU", "D16" } };

        var dict = new DictionaryExpression
        {
            { "LinuxHostVersion", sharedValue },
            { "WindowsHostVersion", sharedValue }
        };

        var yaml = SharplinerSerializer.Serialize(dict);
        
        // Ensure no YAML anchors (&) or aliases (*) are present
        yaml.Should().NotContain("&o");
        yaml.Should().NotContain("*o");
        
        // Ensure both values have the full content
        yaml.ToLower().Should().Contain("linuxhostversion:");
        yaml.ToLower().Should().Contain("windowshostversion:");
        
        // Count occurrences of "sku:" to ensure it appears twice (once for each reference)
        var skuCount = yaml.ToLower().Split("sku:").Length - 1;
        skuCount.Should().Be(2, "SKU should be serialized twice, once for each dictionary entry");
    }
}
