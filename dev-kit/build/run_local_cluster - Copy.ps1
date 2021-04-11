chcp 65001

if(((Get-Module -ListAvailable *) | select -ExpandProperty Name) -notcontains 'powershell-yaml' )
{
	Write-Host -BackgroundColor Red ('ERROR! Have run command: Install-Module -Name powershell-yaml')
	return;
}

$root_folder = Get-Location
$helm_charts_folder=Join-Path -Path $($root_folder) -ChildPath "\build\deploy\local"

$app_manifest = Get-Content (Join-Path -Path $($root_folder) -ChildPath "\build\app.yaml") | ConvertFrom-Yaml

Write-Host Build solution...
dotnet build

Write-Host Application name: $app_manifest["app"]["name"]
foreach($item in $app_manifest["app"]['services'])
{
	Write-Host \n\t---PUBLISH SERVICE---

	Write-Host Service name: $($item.name)
	Write-Host registry: $($item.registry)
	Write-Host tag: $($item.tag)
	Write-Host dockerfile directory: $($item.dockerfileDir)
	Write-Host helm directory: $($item.helmDir)

	$imageName = $item.registry + '/' + $item.name + ':' + $item.tag
	minikube docker-env | Invoke-Expression
	docker build -t $imageName $item.dockerfileDir
	$helmName = $item.registry + '-' + $item.name
	$helmChart = $helm_charts_folder + '\' + $item.name
	helm install $helmName $helmChart 

	Write-Host \n\t---SERVICE PUBLISHED---
}

 minikube service list