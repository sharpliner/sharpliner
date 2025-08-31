# Sharpliner - Azure DevOps Pipeline Definition Library

Sharpliner is a .NET library that enables developers to define Azure DevOps pipelines using C# instead of YAML. The library provides type-safe pipeline definitions with IntelliSense support.

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Build and Setup Process
**NEVER CANCEL any build commands - they can take up to 2 minutes with cold cache. Set timeouts to 180+ seconds minimum.**

**Quick Start**: Use the provided build script to install prerequisites and build the repository:
```bash
./build.sh
# This script will:
# - Install .NET SDK if needed
# - Set up PATH
# - Create necessary directories  
# - Build main library and CI project
# - Pack E2E test library
# - Build entire solution
```

**Manual Steps** (if you need more control):

1. **Build main library** (takes up to 2 minutes with cold cache):
   ```bash
   dotnet build src/Sharpliner/Sharpliner.csproj
   # NEVER CANCEL - takes up to 2 minutes with cold cache, set 180+ second timeout
   ```

2. **Create local NuGet package** for CI project (takes ~3 seconds):
   ```bash
   /home/runner/work/sharpliner/sharpliner/eng/scripts/build-dependencies.sh
   # OR manually:
   dotnet pack src/Sharpliner/Sharpliner.csproj -p:PackageVersion=43.43.43 -c Release
   # Creates: artifacts/package/release/Sharpliner.43.43.43.nupkg
   ```

3. **Build CI project** (takes ~2 seconds):
   ```bash
   dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
   # NEVER CANCEL - completes in 2 seconds, set 180+ second timeout
   # This project uses Sharpliner itself to define the CI pipelines
   ```

4. **Run unit tests** (takes ~9 seconds, some tests may fail with path verification issues - this is expected):
   ```bash
   dotnet test tests/Sharpliner.Tests/Sharpliner.Tests.csproj
   # NEVER CANCEL - completes in 9 seconds, set 180+ second timeout
   # Some path-related test failures are expected and not critical
   ```

5. **Build E2E tests** (takes ~2 seconds each):
   ```bash
   dotnet build tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj
   dotnet build tests/E2E.Tests/ProjectUsingTheLibrary/E2E.Tests.ProjectUsingTheLibrary.csproj
   dotnet build tests/E2E.Tests/ProjectUsingTheLibraryNuGet/E2E.Tests.ProjectUsingTheLibraryNuGet.csproj
   # NEVER CANCEL - each completes in 2 seconds, set 180+ second timeout
   ```

### Validation Scenarios
**ALWAYS run these validation steps after making changes to ensure functionality:**

1. **Core functionality validation**:
   ```bash
   # Build the main library
   dotnet build src/Sharpliner/Sharpliner.csproj
   
   # Create and verify NuGet package exists
   /home/runner/work/sharpliner/sharpliner/eng/scripts/build-dependencies.sh
   ls -la artifacts/package/release/Sharpliner.43.43.43.nupkg  # Should exist
   
   # Build CI project (validates package works)
   dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
   ```

2. **Public API validation** (CRITICAL - must pass):
   ```bash
   dotnet test tests/Sharpliner.Tests/Sharpliner.Tests.csproj --filter "PublicApisHaventChangedTest"
   # If fails: Review API changes, accept if intentional by copying .received.txt to .verified.txt
   ```

3. **Documentation generation validation** (takes ~2 seconds):
   ```bash
   dotnet run --project eng/DocsGenerator/DocsGenerator.csproj -- FailIfChanged=false
   # NEVER CANCEL - completes in 2 seconds, set 180+ second timeout
   # Verifies all documentation templates generate correctly
   ```

4. **E2E validation**:
   ```bash
   # Pack E2E library that uses Sharpliner
   dotnet pack tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj -p:PackageVersion=43.43.43 -c:release
   
   # Build project that references the library
   dotnet build tests/E2E.Tests/ProjectUsingTheLibrary/E2E.Tests.ProjectUsingTheLibrary.csproj
   ```

## Repository Structure

```
.
├── artifacts/              # All build outputs (auto-generated)
├── docs/                   # Documentation (generated from templates)
├── eng/                    # CI/CD for the repository
│   ├── Sharpliner.CI/      # C# pipeline definitions using Sharpliner itself
│   ├── DocsGenerator/      # Tool to generate documentation markdown
│   ├── scripts/            # Build and setup scripts
│   └── pipelines/          # Generated YAML pipelines
├── src/
│   └── Sharpliner/         # Main Sharpliner library (.NET package)
├── tests/
│   ├── E2E.Tests/          # End-to-end tests using Sharpliner NuGet
│   └── Sharpliner.Tests/   # Unit tests for main library
└── Sharpliner.sln          # Main solution file
```

## Key Projects

1. **Sharpliner** (`src/Sharpliner/`): Main library that converts C# pipeline definitions to YAML
2. **Sharpliner.CI** (`eng/Sharpliner.CI/`): Meta project - uses Sharpliner to define Sharpliner's own CI pipelines  
3. **DocsGenerator** (`eng/DocsGenerator/`): Generates documentation from templates
4. **E2E.Tests** (`tests/E2E.Tests/`): Tests that validate real-world usage scenarios

## Development Workflow

### Making Changes to Sharpliner
1. Make your changes in `src/Sharpliner/`
2. **ALWAYS** run validation after changes:
   ```bash
   # Build and test changes
   dotnet build src/Sharpliner/Sharpliner.csproj
   
   # CRITICAL: Test Public API changes
   dotnet test tests/Sharpliner.Tests/Sharpliner.Tests.csproj --filter "PublicApisHaventChangedTest"
   # If test fails, review API changes and accept if intentional
   
   # Clean and rebuild CI project to test with changes
   dotnet clean eng/Sharpliner.CI/Sharpliner.CI.csproj
   /home/runner/work/sharpliner/sharpliner/eng/scripts/build-dependencies.sh
   dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
   
   # Run docs generator to ensure no breaking changes
   dotnet run --project eng/DocsGenerator/DocsGenerator.csproj -- FailIfChanged=false
   ```

### Testing Changes
- Unit tests: `dotnet test tests/Sharpliner.Tests/Sharpliner.Tests.csproj` (some path-related failures expected)
- E2E tests: Build projects in `tests/E2E.Tests/` to validate package usage
- Integration test: Build `eng/Sharpliner.CI/` project successfully
- **Public API test**: `dotnet test --filter "PublicApisHaventChangedTest"` - **MUST PASS** before merging

## Public API Management

**CRITICAL**: Sharpliner is a public-facing library and its public API is strictly controlled and tracked.

### Public API Export Test
- **Test**: `PublicApisHaventChangedTest` in `tests/Sharpliner.Tests/PublicApiChangeTest.cs`
- **Purpose**: Ensures no unintended changes to the public API surface
- **Verified file**: `tests/Sharpliner.Tests/PublicApiExport.txt.verified.txt` contains the expected public API
- **Framework**: Uses Verify.Xunit framework for snapshot testing

### When the Test Fails
The test fails when:
1. **New public members** are added (classes, methods, properties, etc.)
2. **Existing public members** are modified (signatures, visibility, etc.)
3. **Public members are removed** (breaking changes)

Failure indicates API changes that need **explicit approval** from code reviewers.

### Handling API Changes

#### For Intentional API Changes:
1. **Review the diff** between `.verified.txt` and `.received.txt` files:
   ```bash
   diff tests/Sharpliner.Tests/PublicApiExport.txt.verified.txt tests/Sharpliner.Tests/PublicApiExport.txt.received.txt
   ```

2. **Validate changes are intended**:
   - Are new public members necessary and well-designed?
   - Do changes maintain backward compatibility?
   - Are breaking changes justified and documented?

3. **Accept changes** by copying received to verified:
   ```bash
   cp tests/Sharpliner.Tests/PublicApiExport.txt.received.txt tests/Sharpliner.Tests/PublicApiExport.txt.verified.txt
   ```

4. **Commit the updated verified file** along with your changes

#### For Unintended API Changes:
- **Review your code** to ensure only intended members are public
- **Make internal/private** any members that shouldn't be public API
- **Run the test again** to confirm it passes

### Best Practices
- **Minimize public API surface** - only expose what's necessary
- **Design public APIs carefully** - they're hard to change later
- **Consider backward compatibility** - breaking changes affect users
- **Document significant API changes** in commit messages and PR descriptions
- **Always run the PublicAPI test** after making changes to src/Sharpliner/

### Validation Command
```bash
# Run ONLY the Public API test
dotnet test tests/Sharpliner.Tests/Sharpliner.Tests.csproj --filter "PublicApisHaventChangedTest"
```

## Critical Notes

- **NEVER CANCEL builds/tests** - operations can take up to 2 minutes with cold cache
- **Always set timeouts to 180+ seconds** for any build/test commands  
- **The CI project requires the local NuGet package** - run `eng/scripts/build-dependencies.sh` first
- **Some unit test failures are expected** due to path verification in test environment
- **Case sensitivity matters** for Release vs release folder names in build output
- **Multi-targeting**: The library targets net6.0, net7.0, net8.0, net9.0, and net10.0
- **Clean builds**: When making library changes, clean the CI project before rebuilding
- **Public API changes require explicit approval** - update the verified file when intended