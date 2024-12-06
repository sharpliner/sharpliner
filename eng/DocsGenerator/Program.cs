using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DocsGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var templatePath = GetRelativeToGitRoot("eng/DocsGenerator/README.template.md");

        var builder = new StringBuilder();
        using var sr = new StreamReader(templatePath);
        while (sr.ReadLine() is string line)
        {
            if (!line.StartsWith("[!code-"))
            {
                builder.AppendLine(line);
                continue;
            }

            var start = line.IndexOf('(');
            var end = line.IndexOf('#');
            var filepath = line.Substring(start + 1, end - start - 1);
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
                var language = line["[!code-".Length..line.IndexOf("[]")];

                var codeSnippet = GetCodeSnippet(filepath, startLine, endLine, language);
                builder.Append(codeSnippet);
            }
            else
            {
                var codeSnippet = GetCodeSnippet(filepath, lineRangesOrRegion, line["[!code-".Length..line.IndexOf("[]")]);
                builder.Append(codeSnippet);
            }
        }

        var existingReadme = File.ReadAllText(GetRelativeToGitRoot("README.md"));
        var newReadme = builder.ToString();

        if (args.Contains("FailIfChanged=true"))
        {
            if (!existingReadme.Equals(newReadme))
            {
                LogError("README.md has changed. Please run `dotnet run --project .\\eng\\DocsGenerator\\` to update it.");
                Environment.Exit(1);
            }
        }

        File.WriteAllText(GetRelativeToGitRoot("README.md"), newReadme);
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

    private static string GetCodeSnippet(string filepath, int startLine, int endLine, string language)
    {
        var lines = File.ReadAllLines(GetRelativeToGitRoot(filepath));
        var snippet = new StringBuilder();
        snippet.AppendLine($"```{language}");
        var indentation = lines[startLine - 1].Length - lines[startLine - 1].TrimStart().Length;
        for (var i = startLine - 1; i < endLine; i++)
        {
            var line = lines[i].Length < indentation ? lines[i] : lines[i].Substring(indentation);
            snippet.AppendLine(line);
        }

        snippet.AppendLine("```");
        return snippet.ToString();
    }

    private static string GetCodeSnippet(string filepath, string region, string language)
    {
        var lines = File.ReadAllLines(GetRelativeToGitRoot(filepath));
        var snippet = new StringBuilder();
        snippet.AppendLine($"```{language}");

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Equals($"#region {region}"))
            {
                i++;
                var indentation = lines[i].Length - lines[i].TrimStart().Length;
                do
                {
                    var line = lines[i].Length < indentation ? lines[i] : lines[i].Substring(indentation);
                    ++i;
                    if (line is "\"\"\"")
                    {
                        continue;
                    }

                    snippet.AppendLine(line);
                } while (!lines[i].Equals("#endregion"));

                snippet.AppendLine("```");
                return snippet.ToString();
            }
        }

        throw new ArgumentOutOfRangeException($"Region {region} not found in {filepath}");
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
