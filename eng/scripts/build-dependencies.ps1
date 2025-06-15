$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

if (-not(Test-Path "$repo_root/artifacts")) {
    New-Item -Path "$repo_root" -Name "artifacts" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/package")) {
    New-Item -Path "$repo_root/artifacts" -Name "package" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/package/release")) {
    New-Item -Path "$repo_root/artifacts/package" -Name "release" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/package/release/Sharpliner.43.43.43.nupkg")) {
    Write-Host "Building Sharpliner nupkg for Sharpliner.CI..."
    dotnet pack --nologo "$repo_root/src/Sharpliner/Sharpliner.csproj" -p:PackageVersion=43.43.43 -c:Release
}
