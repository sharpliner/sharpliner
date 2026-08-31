using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AndroidSigningTaskTests
{
    [Fact]
    public Task Serialize_AndroidSigning_V3_Defaults_Test()
    {
        var task = new AndroidSigningTaskV3();
        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AndroidSigning_V3_Custom_Test()
    {
        var task = new AndroidSigningTaskV3
        {
            ApkFiles = "**/release/*.apk",
            ApkSign = true,
            ApksignerKeystoreFile = "android-release.keystore",
            ApksignerKeystorePassword = "$(keystorePassword)",
            ApksignerKeystoreAlias = "release",
            ApksignerKeyPassword = "$(keyPassword)",
            ApksignerVersion = "35.0.0",
            ApksignerArguments = "--verbose --v4-signing-enabled false",
            ApksignerFile = "/opt/android/sdk/build-tools/35.0.0/apksigner",
            Zipalign = true,
            ZipalignVersion = "35.0.0",
            ZipalignFile = "/opt/android/sdk/build-tools/35.0.0/zipalign",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AndroidSigning_V3_Without_Signing_Test()
    {
        var task = new AndroidSigningTaskV3
        {
            Files = "**/*.apk",
            ApkSign = false,
            Zipalign = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AndroidSigning_Default_Alias_Task_Test()
    {
        var task = new AndroidSigningTask
        {
            Files = "**/*.apk",
            ApkSign = true,
            KeystoreFile = "keystore.jks",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

#pragma warning disable CS0618
    [Fact]
    public Task Serialize_AndroidSigning_V2_Custom_Test()
    {
        var task = new AndroidSigningTaskV2
        {
            ApkFiles = "**/legacy/*.apk",
            JarSign = true,
            JarsignerKeystoreFile = "legacy-release.keystore",
            JarsignerKeystorePassword = "$(legacyKeystorePassword)",
            JarsignerKeystoreAlias = "legacy",
            JarsignerKeyPassword = "$(legacyKeyPassword)",
            JarsignerArguments = "-verbose -sigalg SHA256withRSA -digestalg SHA-256",
            Zipalign = true,
            ZipalignFile = "/opt/android/sdk/build-tools/31.0.0/zipalign",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
#pragma warning restore CS0618
}
