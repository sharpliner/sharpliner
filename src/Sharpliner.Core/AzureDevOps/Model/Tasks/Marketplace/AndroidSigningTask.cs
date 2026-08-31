using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the latest Android signing task in Azure DevOps pipelines.
/// See <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/android-signing-v3">AndroidSigning@3 task reference</see>.
/// </summary>
public record AndroidSigningTask : AndroidSigningTaskV3;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/android-signing-v3">AndroidSigning@3</see>
/// task that signs APKs with <c>apksigner</c> and optionally runs <c>zipalign</c>.
/// </summary>
/// <remarks>
/// Official task spec:
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AndroidSigningV3/task.json">AndroidSigningV3/task.json</see>.
/// For valid signing configuration, use the <see cref="AndroidSigningTaskBuilder"/> fluent API.
/// </remarks>
public record AndroidSigningTaskV3 : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidSigningTaskV3"/> class.
    /// </summary>
    public AndroidSigningTaskV3() : base("AndroidSigning@3")
    {
    }

    /// <summary>
    /// Relative path from repo root to APK files to sign. Supports wildcards.
    /// Default value: <c>**/*.apk</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> Files
    {
        get => GetExpression<string>("files", "**/*.apk")!;
        init => SetProperty("files", value);
    }

    /// <summary>
    /// Alias for <see cref="Files"/>. Emits <c>files</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ApkFiles
    {
        get => Files;
        init => SetProperty("files", value);
    }

    /// <summary>
    /// Sign the APK with <c>apksigner</c>.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool> ApkSign
    {
        get => GetExpression<bool>("apksign", true)!;
        init => SetProperty("apksign", value);
    }

    /// <summary>
    /// Keystore file uploaded to Secure Files. Required when <see cref="ApkSign"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystoreFile
    {
        get => GetExpression<string>("keystoreFile");
        init => SetProperty("keystoreFile", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystoreFile"/>. Emits <c>keystoreFile</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerKeystoreFile
    {
        get => KeystoreFile;
        init => SetProperty("keystoreFile", value);
    }

    /// <summary>
    /// Password for <see cref="KeystoreFile"/>. Use secret variables.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystorePass
    {
        get => GetExpression<string>("keystorePass");
        init => SetProperty("keystorePass", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystorePass"/>. Emits <c>keystorePass</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerKeystorePassword
    {
        get => KeystorePass;
        init => SetProperty("keystorePass", value);
    }

    /// <summary>
    /// Alias in <see cref="KeystoreFile"/> that identifies the key pair.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystoreAlias
    {
        get => GetExpression<string>("keystoreAlias");
        init => SetProperty("keystoreAlias", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystoreAlias"/>. Emits <c>keystoreAlias</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerKeystoreAlias
    {
        get => KeystoreAlias;
        init => SetProperty("keystoreAlias", value);
    }

    /// <summary>
    /// Password for <see cref="KeystoreAlias"/> key entry. Use secret variables.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeyPass
    {
        get => GetExpression<string>("keyPass");
        init => SetProperty("keyPass", value);
    }

    /// <summary>
    /// Alias for <see cref="KeyPass"/>. Emits <c>keyPass</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerKeyPassword
    {
        get => KeyPass;
        init => SetProperty("keyPass", value);
    }

    /// <summary>
    /// Android SDK build-tools version used to resolve <c>apksigner</c>.
    /// Default value: <c>latest</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ApksignerVersion
    {
        get => GetExpression<string>("apksignerVersion", "latest")!;
        init => SetProperty("apksignerVersion", value);
    }

    /// <summary>
    /// Extra <c>apksigner</c> command arguments.
    /// Default value: <c>--verbose</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ApksignerArguments
    {
        get => GetExpression<string>("apksignerArguments", "--verbose")!;
        init => SetProperty("apksignerArguments", value);
    }

    /// <summary>
    /// Optional explicit path to the <c>apksigner</c> executable.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerLocation
    {
        get => GetExpression<string>("apksignerLocation");
        init => SetProperty("apksignerLocation", value);
    }

    /// <summary>
    /// Alias for <see cref="ApksignerLocation"/>. Emits <c>apksignerLocation</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ApksignerFile
    {
        get => ApksignerLocation;
        init => SetProperty("apksignerLocation", value);
    }

    /// <summary>
    /// Run <c>zipalign</c> on APK packages.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool> Zipalign
    {
        get => GetExpression<bool>("zipalign", true)!;
        init => SetProperty("zipalign", value);
    }

    /// <summary>
    /// Android SDK build-tools version used to resolve <c>zipalign</c>.
    /// Default value: <c>latest</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ZipalignVersion
    {
        get => GetExpression<string>("zipalignVersion", "latest")!;
        init => SetProperty("zipalignVersion", value);
    }

    /// <summary>
    /// Optional explicit path to the <c>zipalign</c> executable.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ZipalignLocation
    {
        get => GetExpression<string>("zipalignLocation");
        init => SetProperty("zipalignLocation", value);
    }

    /// <summary>
    /// Alias for <see cref="ZipalignLocation"/>. Emits <c>zipalignLocation</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ZipalignFile
    {
        get => ZipalignLocation;
        init => SetProperty("zipalignLocation", value);
    }
}

/// <summary>
/// Represents the deprecated
/// <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/android-signing-v2">AndroidSigning@2</see>
/// task that signs APKs with <c>jarsigner</c>.
/// </summary>
/// <remarks>
/// Official task spec:
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AndroidSigningV2/task.json">AndroidSigningV2/task.json</see>.
/// Azure DevOps marks this major as deprecated; prefer <see cref="AndroidSigningTaskV3"/>.
/// </remarks>
[Obsolete("AndroidSigning@2 is deprecated in Azure DevOps. Prefer AndroidSigning@3.")]
public record AndroidSigningTaskV2 : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidSigningTaskV2"/> class.
    /// </summary>
    public AndroidSigningTaskV2() : base("AndroidSigning@2")
    {
    }

    /// <summary>
    /// Relative path from repo root to APK files to sign. Supports wildcards.
    /// Default value: <c>**/*.apk</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> Files
    {
        get => GetExpression<string>("files", "**/*.apk")!;
        init => SetProperty("files", value);
    }

    /// <summary>
    /// Alias for <see cref="Files"/>. Emits <c>files</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> ApkFiles
    {
        get => Files;
        init => SetProperty("files", value);
    }

    /// <summary>
    /// Sign the APK with <c>jarsigner</c>.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool> JarSign
    {
        get => GetExpression<bool>("jarsign", true)!;
        init => SetProperty("jarsign", value);
    }

    /// <summary>
    /// Keystore file uploaded to Secure Files. Required when <see cref="JarSign"/> is true.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystoreFile
    {
        get => GetExpression<string>("keystoreFile");
        init => SetProperty("keystoreFile", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystoreFile"/>. Emits <c>keystoreFile</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JarsignerKeystoreFile
    {
        get => KeystoreFile;
        init => SetProperty("keystoreFile", value);
    }

    /// <summary>
    /// Password for <see cref="KeystoreFile"/>. Use secret variables.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystorePass
    {
        get => GetExpression<string>("keystorePass");
        init => SetProperty("keystorePass", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystorePass"/>. Emits <c>keystorePass</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JarsignerKeystorePassword
    {
        get => KeystorePass;
        init => SetProperty("keystorePass", value);
    }

    /// <summary>
    /// Alias in <see cref="KeystoreFile"/> that identifies the key pair.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeystoreAlias
    {
        get => GetExpression<string>("keystoreAlias");
        init => SetProperty("keystoreAlias", value);
    }

    /// <summary>
    /// Alias for <see cref="KeystoreAlias"/>. Emits <c>keystoreAlias</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JarsignerKeystoreAlias
    {
        get => KeystoreAlias;
        init => SetProperty("keystoreAlias", value);
    }

    /// <summary>
    /// Password for <see cref="KeystoreAlias"/> key entry. Use secret variables.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeyPass
    {
        get => GetExpression<string>("keyPass");
        init => SetProperty("keyPass", value);
    }

    /// <summary>
    /// Alias for <see cref="KeyPass"/>. Emits <c>keyPass</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? JarsignerKeyPassword
    {
        get => KeyPass;
        init => SetProperty("keyPass", value);
    }

    /// <summary>
    /// Extra <c>jarsigner</c> command arguments.
    /// Default value: <c>-verbose -sigalg MD5withRSA -digestalg SHA1</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> JarsignerArguments
    {
        get => GetExpression<string>("jarsignerArguments", "-verbose -sigalg MD5withRSA -digestalg SHA1")!;
        init => SetProperty("jarsignerArguments", value);
    }

    /// <summary>
    /// Run <c>zipalign</c> on APK packages.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool> Zipalign
    {
        get => GetExpression<bool>("zipalign", true)!;
        init => SetProperty("zipalign", value);
    }

    /// <summary>
    /// Optional explicit path to the <c>zipalign</c> executable.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ZipalignLocation
    {
        get => GetExpression<string>("zipalignLocation");
        init => SetProperty("zipalignLocation", value);
    }

    /// <summary>
    /// Alias for <see cref="ZipalignLocation"/>. Emits <c>zipalignLocation</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ZipalignFile
    {
        get => ZipalignLocation;
        init => SetProperty("zipalignLocation", value);
    }
}
