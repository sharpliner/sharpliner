using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DocsGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var failIfChanged = args.Contains("FailIfChanged=true");

        var resources = new (string Template, string Output)[]
        {
            ("eng/DocsGenerator/README.template.md", "README.md"),
            ("eng/DocsGenerator/GettingStarted.template.md", "docs/AzureDevOps/GettingStarted.md"),
        };

        foreach (var (template, output) in resources)
        {
            RenderMarkdownTemplate(template, output, failIfChanged);
        }
    }

    private static void RenderMarkdownTemplate(string template, string output, bool failIfChanged)
    {
        var templatePath = GetRelativeToGitRoot(template);

        var builder = new StringBuilder()
            .AppendLine("<!--")
            .AppendLine(@"This is a generated file by the tool eng/DocsGenerator/Program.cs, Do not edit it manually.")
            .AppendLine("-->")
            .AppendLine();

        using var sr = new StreamReader(templatePath);
        while (sr.ReadLine() is string line)
        {
            if (!line.StartsWith("[!code-"))
            {
                builder.AppendLine(line);
                continue;
            }

            var start = line.IndexOf('(');
            var partialFileSnippet = line.Contains('#');
            var end = partialFileSnippet ? line.IndexOf('#') : line.Length - ")]".Length;
            var language = line["[!code-".Length..line.IndexOf("[]")];
            var filepath = line.Substring(start + 1, end - start - 1);
            var fileLines = File.ReadAllLines(GetRelativeToGitRoot(filepath));

            if (!partialFileSnippet)
            {
                var codeSnippet = GetCodeSnippet(fileLines, language);
                builder.Append(codeSnippet);
                continue;
            }

            var lineRangesOrRegion = line.Substring(end + 1, line.Length - end - 1 - ")]".Length);
            if (lineRangesOrRegion.Contains("-L"))
            {
                var lines = lineRangesOrRegion.Split('-');
                if (lines.Length != 2)
                {
                    throw new ArgumentOutOfRangeException($"Invalid line range in {line}");
                }

                var startLine = int.Parse(lines[0].Substring("L".Length));
                var endLine = int.Parse(lines[1].Substring("L".Length));

                var codeSnippet = GetCodeSnippet(fileLines, startLine, endLine, language);
                builder.Append(codeSnippet);
            }
            else
            {
                var codeSnippet = GetCodeSnippet(fileLines, lineRangesOrRegion, language);
                builder.Append(codeSnippet);
            }
        }

        var existingReadme = File.ReadAllText(GetRelativeToGitRoot(output));
        var newReadme = builder.ToString();

        if (failIfChanged)
        {
            if (!existingReadme.Equals(newReadme))
            {
                LogError($"{output} has changed. Please run `dotnet run --project .\\eng\\DocsGenerator\\` to update it.");
                Environment.Exit(1);
            }
        }

        File.WriteAllText(GetRelativeToGitRoot(output), newReadme);
    }

    private static void LogError(string message)
    {
        if (Environment.GetEnvironmentVariable("TF_BUILD") is not null)
        {
            Console.WriteLine($"##vso[task.logissue type=error]{message}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    private static string GetCodeSnippet(string[] lines, int startLine, int endLine, string language)
    {
        return GetCodeSnippet(lines[(startLine - 1)..endLine], language);
    }

    private static string GetCodeSnippet(string[] lines, string region, string language)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Equals($"#region {region}"))
            {
                var relevantLines = new List<string>();
                i++;
                do
                {
                    var line = lines[i];
                    ++i;
                    if (line.EndsWith("\"\"\""))
                    {
                        continue;
                    }

                    relevantLines.Add(line);
                } while (!lines[i].Equals("#endregion"));
                return GetCodeSnippet(relevantLines.ToArray(), language);
            }
        }

        throw new ArgumentOutOfRangeException($"Region {region} not found in file");
    }

    private static string GetCodeSnippet(string[] lines, string language)
    {
        var snippet = new StringBuilder();
        snippet.AppendLine($"```{language}");

        var indentation = lines[0].Length - lines[0].TrimStart().Length;

        foreach (var line in lines)
        {
            var normalizedLine = line.Length < indentation ? line : line.Substring(indentation);
            snippet.AppendLine(normalizedLine);
        }

        snippet.AppendLine("```");
        return snippet.ToString();
    }

    private static string GetRelativeToGitRoot(string path)
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (!Directory.Exists(Path.Combine(currentDir.FullName, ".git")))
        {
            currentDir = currentDir.Parent;

            if (currentDir == null)
            {
                throw new Exception($"Failed to find git repository in {Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName}");
            }
        }

        return Path.Combine(currentDir.FullName, path);
    }
}
