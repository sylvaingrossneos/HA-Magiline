[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$HostName,

    [ValidateRange(1, 65535)]
    [int]$Port = 11000
)

$ErrorActionPreference = 'Stop'
$uri = "http://$HostName`:$Port/api/v1/pool/local"

try {
    Invoke-RestMethod -Method Get -Uri $uri -TimeoutSec 10
}
catch {
    throw "Unable to read Magiline state from $uri. $($_.Exception.Message)"
}
