using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class PublishSymbolsTaskTests
{
    [Fact]
    public Task Serialize_Azure_Artifacts_Task_Test()
    {
        var task = new PublishSymbolsTeamServicesTask("**/bin/**/*.pdb")
        {
            SymbolsFolder = "$(Build.BinariesDirectory)",
            IndexableFileFormats = IndexableFileFormats.Pdb,
            SymbolExpirationInDays = 30,
            DetailedLog = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_File_Share_Task_Test()
    {
        var task = new PublishSymbolsFileShareTask("**/bin/**/*.pdb", @"\\myshare\symbols")
        {
            CompressSymbols = true,
            SymbolsProduct = "MyProduct",
            SymbolsVersion = "1.2.3",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Index_Only_Task_Test()
    {
        var task = new PublishSymbolsIndexSourcesTask("**/bin/**/*.pdb")
        {
            Manifest = "$(Build.SourcesDirectory)/symbols.manifest",
            TreatNotIndexedAsWarning = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
