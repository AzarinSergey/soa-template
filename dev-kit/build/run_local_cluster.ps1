﻿chcp 65001

if(((Get-Module -ListAvailable *) | Select-Object -ExpandProperty Name) -notcontains 'powershell-yaml' )
{
	Write-Host -BackgroundColor Red ('ERROR! Run the command: Install-Module -Name powershell-yaml')
	return;
}

$root_folder = Get-Location
$helm_charts_folder=Join-Path -Path $($root_folder) -ChildPath "\build\deploy\local"
$app_manifest = Get-Content (Join-Path -Path $($root_folder) -ChildPath "\build\app.yaml") | ConvertFrom-Yaml
$app_local_env = Get-Content $(Join-Path -Path $($root_folder) -ChildPath "\build\deploy\local\infrastructure.env") | ConvertFrom-StringData
$local_user_ipaddress=$((Get-NetIPConfiguration | Where-Object {$_.IPv4DefaultGateway -ne $null -and $_.NetAdapter.status -ne "Disconnected"}).IPv4Address.IPAddress)

Write-Host Build solution...
dotnet build

minikube docker-env | Invoke-Expression

foreach($item in $app_manifest["app"]['services'])
{
	$imageName = $item.registry + '/' + $item.name + ':' + $item.tag
	$helmName = $item.registry + '-' + $item.name
	$helmChart = $helm_charts_folder + '\' + $item.name

	Write-Host \n\t---PUBLISH SERVICE--- $imageName

	minikube docker-env | Invoke-Expression
	docker build -t $imageName $item.projectDir --no-cache --label "tmp"

	helm install $helmName $helmChart  `
	--set infrastructure.redis.host=$($local_user_ipaddress) `
	--set infrastructure.redis.port=$($app_local_env.REDIS_PORT) `
	--set infrastructure.postgres.host=$($local_user_ipaddress) `
	--set infrastructure.postgres.port=$($app_local_env.POSTGRES_PORT) `

	Write-Host \n\t---SERVICE PUBLISHED--- $imageName
}

 minikube service list

 minikube docker-env --unset | Invoke-Expression