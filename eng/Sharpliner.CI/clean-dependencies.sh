#!/bin/bash

# Clean cache
dotnet nuget locals all --list | cut -d " " -f2 | xargs -i sh -c 'rm -rfv "{}/sharpliner"*'

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

# Remove Sharpliner .nupkg packages
rm -rf "$repo_root/artifacts/packages"
