$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

dotnet pack "$repo_root/src/Sharpliner/Sharpliner.csproj"
dotnet pack "$repo_root/src/Sharpliner.Tools/Sharpliner.Tools.csproj"
