chcp 65001

if(((Get-Module -ListAvailable *) | Select-Object -ExpandProperty Name) -notcontains 'powershell-yaml' )
{
	Write-Host -BackgroundColor Red ('ERROR! Run the command: Install-Module -Name powershell-yaml')
	return;
}

function ConvertTo-StringData {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, Position = 0, ValueFromPipeline)]
        [HashTable[]]$HashTable
    )
    process {
        $SB = [System.Text.StringBuilder]::new()
        foreach ($item in $HashTable) {
            foreach ($entry in $item.GetEnumerator()) {
                [void]$SB.AppendLine($("{0}={1}" -f $entry.Key, $entry.Value))
            }
        }

        return $SB.ToString()
    }
}

$root_folder = Get-Location
$_env_user = Join-Path -Path $($root_folder) -ChildPath "\.env.user"
$helm_charts_folder=Join-Path -Path $($root_folder) -ChildPath "\build\deploy\local"
$app_manifest = Get-Content (Join-Path -Path $($root_folder) -ChildPath "\build\app.yaml") | ConvertFrom-Yaml

if(!(Test-Path -Path $($_env_user) -PathType Leaf))
{
	Write-Host Create .\.env.user file
	$filedata = @{
        ipaddress=$((Get-NetIPConfiguration | Where-Object {$_.IPv4DefaultGateway -ne $null -and $_.NetAdapter.status -ne "Disconnected"}).IPv4Address.IPAddress)
        #trololoKey='tralalaValue'
    }

    Out-File -FilePath $_env_user -InputObject $(ConvertTo-StringData $filedata)
}

Write-Host Build solution...
dotnet build

Write-Host Application name: $app_manifest["app"]["name"]
foreach($item in $app_manifest["app"]['services'])
{
	Write-Host \n\t---PUBLISH SERVICE---

	Write-Host Service name: $($item.name)
	Write-Host registry: $($item.registry)
	Write-Host tag: $($item.tag)
	Write-Host dockerfile directory: $($item.projectDir)
	Write-Host helm directory: $($item.helmDir)

	$imageName = $item.registry + '/' + $item.name + ':' + $item.tag
	minikube docker-env | Invoke-Expression

	docker build -t $imageName $item.projectDir --no-cache --label "tmp"
	$helmName = $item.registry + '-' + $item.name
	$helmChart = $helm_charts_folder + '\' + $item.name
	helm install $helmName $helmChart 

	Write-Host \n\t---SERVICE PUBLISHED---
}

 minikube service list