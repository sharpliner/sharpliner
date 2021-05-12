$repo_root = Join-Path $PSScriptRoot ".."
$repo_root = Join-Path $repo_root ".."

if (-not(Test-Path "$repo_root/artifacts")) {
    New-Item -Path "$repo_root" -Name "artifacts" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/packages")) {
    New-Item -Path "$repo_root/artifacts" -Name "packages" -ItemType "directory" | out-null
}

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.1.0.0.nupkg")) {
    Write-Host "Building Sharpliner nupkg for Sharpliner.CI..."
    dotnet pack --nologo "$repo_root/src/Sharpliner/Sharpliner.csproj"
}

if (-not(Test-Path "$repo_root/artifacts/packages/Sharpliner.Tools.1.0.0.nupkg")) {
    Write-Host "Building Sharpliner.Tools nupkg for Sharpliner.CI..."
    dotnet pack --nologo "$repo_root/src/Sharpliner.Tools/Sharpliner.Tools.csproj"
}
