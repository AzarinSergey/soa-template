chcp 65001

$root_folder = Get-Location

$app_manifest = Get-Content (Join-Path -Path $($root_folder) -ChildPath "\build\app.yaml") | ConvertFrom-Yaml

Write-Host Application name: $app_manifest["app"]["name"]
foreach($item in $app_manifest["app"]['services'])
{
	Write-Host \n\t---KILL SERVICE---
	
	$helmName = $item.registry + '-' + $item.name

	Write-Host Helm name: $($helmName)

	helm uninstall $helmName 

	Write-Host \n\t---SERVICE KILLED---
}

 minikube service list