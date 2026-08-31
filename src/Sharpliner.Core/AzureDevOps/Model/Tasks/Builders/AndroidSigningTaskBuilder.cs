using System;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Fluent builder for Android signing tasks.
/// </summary>
/// <remarks>
/// See official task references:
/// <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/android-signing-v3">AndroidSigning@3</see> and
/// <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/android-signing-v2">AndroidSigning@2</see>.
/// </remarks>
public class AndroidSigningTaskBuilder
{
    /// <summary>
    /// Gets fluent APIs for <c>AndroidSigning@3</c> (<c>apksigner</c>).
    /// </summary>
    public AndroidSigningV3Builder V3 => new();

    /// <summary>
    /// Gets fluent APIs for deprecated <c>AndroidSigning@2</c> (<c>jarsigner</c>).
    /// </summary>
    public AndroidSigningV2Builder V2 => new();

    internal AndroidSigningTaskBuilder()
    {
    }
}

/// <summary>
/// Fluent entrypoint for <c>AndroidSigning@3</c> configurations.
/// </summary>
public class AndroidSigningV3Builder
{
    /// <summary>
    /// Starts a V3 signing configuration for matching APK files.
    /// </summary>
    /// <param name="files">APK file glob. Default is <c>**/*.apk</c>.</param>
    public AndroidSigningV3ConfigurationBuilder ForApkFiles(AdoExpression<string>? files = null) => new(files ?? "**/*.apk");
}

/// <summary>
/// Fluent builder that enforces a valid V3 signing mode choice.
/// </summary>
public class AndroidSigningV3ConfigurationBuilder(AdoExpression<string> files)
{
    /// <summary>
    /// Creates an <c>AndroidSigning@3</c> task with APK signing disabled.
    /// </summary>
    public AndroidSigningTaskV3 WithoutSigning() => new()
    {
        Files = files,
        ApkSign = false,
    };

    /// <summary>
    /// Creates an <c>AndroidSigning@3</c> task with APK signing enabled and a required keystore file.
    /// </summary>
    public AndroidSigningTaskV3 SignWithKeystore(AdoExpression<string> keystoreFile)
    {
        ArgumentNullException.ThrowIfNull(keystoreFile);

        return new()
        {
            Files = files,
            ApkSign = true,
            KeystoreFile = keystoreFile,
        };
    }
}

/// <summary>
/// Fluent entrypoint for deprecated <c>AndroidSigning@2</c> configurations.
/// </summary>
public class AndroidSigningV2Builder
{
    /// <summary>
    /// Starts a V2 signing configuration for matching APK files.
    /// </summary>
    /// <param name="files">APK file glob. Default is <c>**/*.apk</c>.</param>
    [System.Obsolete("AndroidSigning@2 is deprecated in Azure DevOps. Prefer AndroidSigning@3.")]
    public AndroidSigningV2ConfigurationBuilder ForApkFiles(AdoExpression<string>? files = null) => new(files ?? "**/*.apk");
}

/// <summary>
/// Fluent builder that enforces a valid V2 signing mode choice.
/// </summary>
public class AndroidSigningV2ConfigurationBuilder(AdoExpression<string> files)
{
    /// <summary>
    /// Creates an <c>AndroidSigning@2</c> task with APK signing disabled.
    /// </summary>
    [System.Obsolete("AndroidSigning@2 is deprecated in Azure DevOps. Prefer AndroidSigning@3.")]
    public AndroidSigningTaskV2 WithoutSigning() => new()
    {
        Files = files,
        JarSign = false,
    };

    /// <summary>
    /// Creates an <c>AndroidSigning@2</c> task with APK signing enabled and a required keystore file.
    /// </summary>
    [System.Obsolete("AndroidSigning@2 is deprecated in Azure DevOps. Prefer AndroidSigning@3.")]
    public AndroidSigningTaskV2 SignWithKeystore(AdoExpression<string> keystoreFile)
    {
        ArgumentNullException.ThrowIfNull(keystoreFile);

        return new()
        {
            Files = files,
            JarSign = true,
            KeystoreFile = keystoreFile,
        };
    }
}
