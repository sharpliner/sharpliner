#!/bin/bash

# Get current path
here="${BASH_SOURCE[0]}"
# resolve $source until the file is no longer a symlink
while [[ -h "$here" ]]; do
  script_root="$( cd -P "$( dirname "$here" )" && pwd )"
  here="$(readlink "$here")"
  # if $here was a relative symlink, we need to resolve it relative to the path where the
  # symlink file was located
  [[ $here != /* ]] && source="$script_root/$here"
done

here="$( cd -P "$( dirname "$here" )" && pwd )"
repo_root="$here/../../"

# Prepare local packages

if [ ! -f "$repo_root/artifacts/packages/Sharpliner.43.43.43.nupkg" ]; then
  mkdir -p "$repo_root/artifacts/packages"
  echo "Building Sharpliner nupkg for Sharpliner.CI..."
  dotnet pack --nologo "$repo_root/src/Sharpliner/Sharpliner.csproj" -p:PackageVersion=43.43.43 -c:Release
fi
