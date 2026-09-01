using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DownloadSecureFileTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new DownloadSecureFileTask("signing-cert.p12");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Custom_Retry_And_Socket_Settings_Test()
    {
        var task = new DownloadSecureFileTask("ca.pem", retryCount: 3, socketTimeout: 60000)
        {
            Name = "secureFileStep",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void OutputSecureFilePath_Requires_Step_Name()
    {
        Assert.Throws<ArgumentException>(() => DownloadSecureFileTask.OutputSecureFilePath(string.Empty));
        Assert.Throws<ArgumentException>(() => DownloadSecureFileTask.OutputSecureFilePath(" "));
    }

    [Fact]
    public void OutputSecureFilePath_Formats_Output_Variable_Reference()
    {
        Assert.Equal("$(secureFileStep.secureFilePath)", DownloadSecureFileTask.OutputSecureFilePath("secureFileStep"));
    }
}
