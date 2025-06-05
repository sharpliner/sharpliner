using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.CI;

class ProjectBuildSteps(string project) : StepLibrary
{
    public const string PackagePath = "artifacts/packages";

    public override List<AdoExpression<Step>> Steps =>
    [
        StepTemplate(Pipelines.TemplateLocation + "install-dotnet-sdk.yml", new()
        {
            { "version", GetSdkVersionFromGlobalJson() }
        }),

        Powershell
            .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
            .DisplayAs($"Create {PackagePath}"),

        DotNet
            .Build(project, includeNuGetOrg: true)
            .DisplayAs("Build"),
    ];

    private static string GetSdkVersionFromGlobalJson()
    {
        var globalJson = File.ReadAllText(GetGlobalJsonPath());
        using var jsonDoc = JsonDocument.Parse(globalJson);
        if (jsonDoc.RootElement.TryGetProperty("sdk", out JsonElement sdk) &&
            sdk.TryGetProperty("version", out JsonElement version))
        {
            return version.GetString()
                ?? throw new Exception("The global.json file does not contain the required 'sdk.version' property.");
        }

        throw new Exception("The global.json file does not contain the required 'sdk.version' property.");
    }

    private static string GetGlobalJsonPath()
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (!Directory.Exists(Path.Combine(currentDir.FullName, ".git")) && !File.Exists(Path.Combine(currentDir.FullName, ".git")))
        {
            currentDir = currentDir.Parent;

            if (currentDir == null)
            {
                throw new Exception($"Failed to find git repository in {Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName}");
            }
        }

        return Path.Combine(currentDir.FullName, "global.json");
    }
}
