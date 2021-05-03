[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [String]$serviceName
)

chcp 65001

if(((Get-Module -ListAvailable *) | Select-Object -ExpandProperty Name) -notcontains 'powershell-yaml' )
{
	Write-Host -BackgroundColor Red ('ERROR! Run the command: Install-Module -Name powershell-yaml')
	return;
}

$root_folder = Get-Location
$helm_charts_folder=Join-Path -Path $($root_folder) -ChildPath "\build\deploy\local"

$app_manifest = Get-Content (Join-Path -Path $($root_folder) -ChildPath "\build\app.yaml") | ConvertFrom-Yaml



#PREPARE VARIABLES
$service_config = $app_manifest["app"]['services'] | Where-Object -Property name -eq $serviceName
$imageRandomTag =  $service_config.tag + '-' + $(New-Guid)
$imageName = $service_config.registry + '/' + $service_config.name + ':' + $imageRandomTag
$helmName = $service_config.registry + '-' + $service_config.name
$helmChart = $helm_charts_folder + '\' + $service_config.name

Write-Host $imageName
Write-Host Service name: $($service_config.name)
Write-Host registry: $($service_config.registry)
Write-Host tag: $($service_config.tag)
Write-Host dockerfile directory: $($service_config.projectDir)
Write-Host helm directory: $($service_config.helmDir)

#BUILD PROJECT
Push-Location $(Join-Path -Path $($root_folder) -ChildPath $service_config.projectDir)
Write-Host Build project...
dotnet build
Pop-Location

#BUILD IMAGE
minikube docker-env | Invoke-Expression
docker build -t $imageName $service_config.projectDir --no-cache --label "tmp"

#UPDATE SERVICE
helm upgrade $helmName $helmChart --debug --set image.tag=$($imageRandomTag)

#DEATTACH CONSOLE FROM MINIKUBE DOCKER ENV
 minikube docker-env --unset | Invoke-Expression

#INFO OUTPUT
helm history $helmName
minikube service list

