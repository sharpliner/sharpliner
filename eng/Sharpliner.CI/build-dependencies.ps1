$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

if (-not(Test-Path "$repo_root/artifacts")) {
    New-Item -Path "$repo_root" -Name "artifacts" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/packages")) {
    New-Item -Path "$repo_root/artifacts" -Name "packages" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.42.42.42.nupkg")) {
    Write-Host "Building Sharpliner nupkg for Sharpliner.CI..."
    dotnet pack --nologo "$repo_root/src/Sharpliner/Sharpliner.csproj" -p:PackageVersion=42.42.42.42
}

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.Tools.42.42.42.nupkg")) {
    Write-Host "Building Sharpliner.Tools nupkg for Sharpliner.CI..."
    dotnet pack --nologo "$repo_root/src/Sharpliner.Tools/Sharpliner.Tools.csproj" -p:PackageVersion=42.42.42.42
}
