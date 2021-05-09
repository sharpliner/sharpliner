# Clean cache
$caches = dotnet nuget locals all --list
Foreach ($cache in $caches) {
    Remove-Item -Recurse "$($cache.Split(" ")[1])/sharpliner*"
}

$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

# Remove Sharpliner .nupkg packages
Remove-Item -Recurse "$repo_root/artifacts/packages/*"
