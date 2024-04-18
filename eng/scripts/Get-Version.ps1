$tag = git tag --points-at HEAD

if ("" -eq "$tag") {
    Write-Host '##vso[task.complete result=Failed;]No git tag with version found'
    exit 1
}

Write-Host "##vso[task.setvariable variable=packageVersion]$tag"
