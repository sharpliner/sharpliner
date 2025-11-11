#!/bin/bash

# Check if .dotnet directory already exists
if [ ! -d "./.dotnet" ]; then
    echo "Installing .NET SDK..."
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.100 --install-dir ./.dotnet
else
    echo ".NET SDK installation directory already exists"
fi

export DOTNET_ROOT=$PWD/.dotnet
export PATH=$DOTNET_ROOT:$PATH

mkdir -p artifacts/package/release
mkdir -p artifacts/packages
dotnet build src/Sharpliner/Sharpliner.csproj
dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
dotnet pack tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj -p:PackageVersion=43.43.43 -c:release
dotnet build Sharpliner.sln
