$tag = git tag --points-at HEAD
if ("" -eq "$tag") {
# TODO do not check it
#    Write-Host '##vso[task.complete result=Failed;]No git tag with version found'
#  exit 1
    $tag = "1.2.3"
}

Write-Host "##vso[task.setvariable variable=packageVersion]$tag"
