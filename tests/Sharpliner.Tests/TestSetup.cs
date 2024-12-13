using System.Runtime.CompilerServices;

namespace Sharpliner.Tests;

public class TestSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        
        Verifier.DerivePathInfo((sourceFile, projectDirectory, type, method) =>
        {
            return new PathInfo(Path.Join(projectDirectory, "Verified"), type.Name, method.Name);
        });
    }
}
