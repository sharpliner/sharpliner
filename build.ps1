#!/usr/bin/env pwsh

$SdkVersion = (Get-Content global.json -Raw | ConvertFrom-Json).sdk.version

# Check if the required .NET SDK already exists
if (!(Test-Path ".\.dotnet\sdk\$SdkVersion")) {
    Write-Host "Installing .NET SDK..."
    
    # Download and run the .NET install script
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile "dotnet-install.ps1"
    
    # Install .NET SDK to local .dotnet directory
    .\dotnet-install.ps1 -Version $SdkVersion -InstallDir ".\.dotnet"
    
    # Clean up the install script
    Remove-Item "dotnet-install.ps1"
} else {
    Write-Host ".NET SDK $SdkVersion installation directory already exists"
}

# Set environment variables
$env:DOTNET_ROOT = Join-Path $PWD ".dotnet"
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"

# Create necessary directories
New-Item -ItemType Directory -Path "artifacts\package\release" -Force | Out-Null
New-Item -ItemType Directory -Path "artifacts\packages" -Force | Out-Null

# Build projects
Write-Host "Building Sharpliner library..."
dotnet build src\Sharpliner\Sharpliner.csproj

Write-Host "Building Sharpliner CI..."
dotnet build eng\Sharpliner.CI\Sharpliner.CI.csproj

Write-Host "Packing E2E test library..."
dotnet pack tests\E2E.Tests\SharplinerLibrary\E2E.Tests.SharplinerLibrary.csproj -p:PackageVersion=43.43.43 -c:release

Write-Host "Building entire solution..."
dotnet build Sharpliner.slnx

Write-Host "Build completed successfully!"
