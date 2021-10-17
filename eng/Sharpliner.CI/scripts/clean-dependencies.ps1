# Clean cache
$caches = dotnet nuget locals all --list
Foreach ($cache in $caches) {
    Remove-Item -Recurse "$($cache.Split(" ")[1])/sharpliner*"
}

$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."
$repo_root = Join-Path $repo_root ".."

# Remove Sharpliner .nupkg packages

if (Test-Path "$repo_root/artifacts/packages") {
    Remove-Item -Recurse "$repo_root/artifacts/packages/*"
}
