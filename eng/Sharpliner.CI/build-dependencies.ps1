$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.1.0.0.nupkg")) {
    dotnet pack "$repo_root/src/Sharpliner/Sharpliner.csproj"
}

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.Tools.1.0.0.nupkg")) {
    dotnet pack "$repo_root/src/Sharpliner.Tools/Sharpliner.Tools.csproj"
}
