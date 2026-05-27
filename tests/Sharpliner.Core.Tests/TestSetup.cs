using System.Runtime.CompilerServices;

namespace Sharpliner.Tests;

public class TestSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Verifier.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            return new PathInfo(
                Path.Join(projectDirectory, "Verified", type.Namespace!.Substring("Sharpliner.Tests".Length).TrimStart('.')), 
                type.Name,
                method.Name);
        });

        // This simplifies adding new tests. see https://github.com/VerifyTests/Verify/blob/main/docs/verify-options.md#autoverify
        // VerifierSettings.AutoVerify();
    }
}
