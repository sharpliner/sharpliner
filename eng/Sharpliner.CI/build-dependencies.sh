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

if [ ! -f "$repo_root/artifacts/packages/Sharpliner.1.0.0.nupkg" ]; then
  dotnet pack "$repo_root/src/Sharpliner/Sharpliner.csproj"
fi

if [ ! -f "$repo_root/artifacts/packages/Sharpliner.Tools.1.0.0.nupkg" ]; then
  dotnet pack "$repo_root/src/Sharpliner.Tools/Sharpliner.Tools.csproj"
fi
