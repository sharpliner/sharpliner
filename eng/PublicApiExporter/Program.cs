using System;
using System.IO;
using PublicApiGenerator;
using Sharpliner;

var assembly = typeof(ISharplinerDefinition).Assembly;
var publicApi = assembly.GeneratePublicApi();
publicApi = publicApi.Substring(publicApi.IndexOf("namespace ")).Trim();
var exportApiPath = Path.Combine(assembly.Location, "..", "..", "..", "..", "..", "tests", "Sharpliner.Tests", "PublicApiExport.txt");
File.WriteAllText(exportApiPath, publicApi);
Console.WriteLine("Public API exported to PublicApiExport.txt");
