$tag = git tag --points-at HEAD
if ("" -eq "$tag") {
  Write-Host '##vso[task.complete result=Failed;]No git tag with version found'
  exit 1
}

$versions = $tag.Split('.')

$major = $versions[0]
$minor = $versions[1]
$patch = $versions[2]

Write-Host "##vso[task.setvariable variable=majorVersion]$major"
Write-Host "##vso[task.setvariable variable=minorVersion]$minor"
Write-Host "##vso[task.setvariable variable=patchVersion]$patch"
