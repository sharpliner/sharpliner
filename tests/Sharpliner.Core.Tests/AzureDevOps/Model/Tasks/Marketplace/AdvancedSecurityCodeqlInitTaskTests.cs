using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AdvancedSecurityCodeqlInitTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Representative_Configuration_Test()
    {
        var task = new AdvancedSecurityCodeqlInitTask(CodeqlLanguage.CSharp, CodeqlLanguage.JavaScript)
        {
            EnableAutomaticCodeQLInstall = true,
            CleanupOldAutomaticInstalls = true,
            QuerySuite = CodeqlQuerySuite.SecurityAndQuality,
            BuildType = CodeqlBuildType.None,
            Ram = "8192",
            Threads = "0",
            CodeqlPathsToIgnore = "generated,legacy",
            CodeqlPathsToInclude = "src,services",
            SourcesFolder = "src",
            LogLevel = CodeqlLogLevel.Debug,
            ConfigFilePath = "/agent/_work/1/s/.github/codeql/codeql-config.yml",
            CodeqlToolsDirectory = "/agent/_work/_tool/CodeQL",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new AdvancedSecurityCodeqlInitTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void WithLanguages_Uses_Allowed_Yaml_Values()
    {
        var task = new AdvancedSecurityCodeqlInitTask().WithLanguages(CodeqlLanguage.Cpp, CodeqlLanguage.Python);

        var languages = ((Sharpliner.AzureDevOps.Expressions.AdoExpression<string>)task.Inputs["languages"]).GetDefinitionValue();
        Assert.Equal("cpp,python", languages);
    }

    [Fact]
    public void Languages_Are_Required_When_Using_Typed_Language_APIs()
    {
        Assert.Throws<ArgumentException>(() => new AdvancedSecurityCodeqlInitTask([]));
        Assert.Throws<ArgumentException>(() => new AdvancedSecurityCodeqlInitTask().WithLanguages());
    }
}
