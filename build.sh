#!/bin/bash

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir ./.dotnet
export DOTNET_ROOT=$PWD/.dotnet
export PATH=$DOTNET_ROOT:$PATH

mkdir -p artifacts/package/release
mkdir -p artifacts/packages
dotnet build src/Sharpliner/Sharpliner.csproj
dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
dotnet build Sharpliner.sln
