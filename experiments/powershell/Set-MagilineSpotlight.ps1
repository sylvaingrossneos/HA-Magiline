[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$HostName,

    [Parameter(Mandatory = $true)]
    [ValidateSet('On', 'Off')]
    [string]$State,

    [ValidateRange(1, 65535)]
    [int]$Port = 11000
)

$ErrorActionPreference = 'Stop'
$wanted = if ($State -eq 'On') { 2 } else { 1 }
$uri = "http://$HostName`:$Port/api/v1/pool/local/spotlight"
$body = @{ mode = @{ wanted = $wanted } } | ConvertTo-Json -Depth 3

if ($PSCmdlet.ShouldProcess($uri, "Set spotlight $State (wanted=$wanted)")) {
    try {
        Invoke-RestMethod -Method Post -Uri $uri -ContentType 'application/json' -Body $body -TimeoutSec 10
    }
    catch {
        throw "Unable to set Magiline spotlight through $uri. $($_.Exception.Message)"
    }
}
