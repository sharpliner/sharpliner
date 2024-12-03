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
        var line = sr.ReadLine();
        while (line is not null)
        {
            if (line.StartsWith("[!code-"))
            {
                var start = line.IndexOf('(');
                var end = line.IndexOf('#');
                var filepath = line.Substring(start + 1, end - start - 1);
                var lines = line.Substring(end + 1, line.Length - end - 1 - ")]".Length).Split('-');

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
                builder.AppendLine(line);
            }

            line = sr.ReadLine();
        }

        var existingReadme = File.ReadAllText(GetRelativeToGitRoot("README.md"));
        var newReadme = builder.ToString();

        if (args.Contains("FailIfChanged=true"))
        {
            if (!existingReadme.Equals(newReadme))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("README.md has changed. Please run `dotnet run --project .\\eng\\DocsGenerator\\` to update it.");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        File.WriteAllText(GetRelativeToGitRoot("README.md"), newReadme);
    }

    private static string GetCodeSnippet(string filepath, int startLine, int endLine, string language)
    {
        var lines = File.ReadAllLines(GetRelativeToGitRoot(filepath));
        var snippet = new StringBuilder();
        snippet.AppendLine($"```{language}");
        var indentation = lines[startLine - 1].Length - lines[startLine - 1].TrimStart().Length;
        for (var i = startLine - 1; i < endLine; i++)
        {
            if (lines[i].Length < indentation)
            {
                snippet.AppendLine(lines[i]);
            }
            else
            {
                snippet.AppendLine(lines[i].Substring(indentation));
            }
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
