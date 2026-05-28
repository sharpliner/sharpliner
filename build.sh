#!/bin/bash

SDK_VERSION=$(grep -oP '"version"\s*:\s*"\K[^"]+' global.json)

# Check if the required .NET SDK already exists
if [ ! -d "./.dotnet/sdk/$SDK_VERSION" ]; then
    echo "Installing .NET SDK..."
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version "$SDK_VERSION" --install-dir ./.dotnet
else
    echo ".NET SDK $SDK_VERSION installation directory already exists"
fi

export DOTNET_ROOT=$PWD/.dotnet
export PATH=$DOTNET_ROOT:$PATH

mkdir -p artifacts/package/release
mkdir -p artifacts/packages
dotnet build src/Sharpliner/Sharpliner.csproj
dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
dotnet pack tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj -p:PackageVersion=43.43.43 -c:release
dotnet build Sharpliner.slnx
