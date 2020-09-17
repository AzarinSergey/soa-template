[CmdletBinding()]
param (
	[Parameter(Mandatory=$true, Position=0)]
    [string]
    $registry
)


docker push $registry/exs
docker push $registry/api-test