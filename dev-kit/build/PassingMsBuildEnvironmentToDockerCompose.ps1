[CmdletBinding()]
param (
	[Parameter(Mandatory=$true, Position=0)]
    [string]
    $AppVariablesFilePath,

    [Parameter(Mandatory=$true, Position=1)]
    [string]
    $MsBuildVariables,

    [Parameter(Mandatory=$true, Position=2)]
    [string]
    $OutputDirectory
)

$srcEvironmentVariables = (Get-Content ($AppVariablesFilePath)) -join  "`r`n"
$additionalEnvironmentVariables = ($MsBuildVariables.Split(',')) -join "`r`n"

Set-Content  $($OutputDirectory + ".env") $(($srcEvironmentVariables,$additionalEnvironmentVariables) -join "`r`n")