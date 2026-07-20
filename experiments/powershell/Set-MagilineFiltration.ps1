[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$HostName,

    [Parameter(Mandatory = $true)]
    [ValidateSet('Auto', 'ForceOn', 'Off')]
    [string]$Mode,

    [ValidateRange(1, 65535)]
    [int]$Port = 11000
)

$ErrorActionPreference = 'Stop'
$wantedByMode = @{
    Auto    = 0
    ForceOn = 1
    Off     = 2
}

$wanted = $wantedByMode[$Mode]
$uri = "http://$HostName`:$Port/api/v1/pool/local/configure-filtration"
$body = @{ mode = @{ wanted = $wanted } } | ConvertTo-Json -Depth 3

if ($PSCmdlet.ShouldProcess($uri, "Set filtration mode $Mode (wanted=$wanted)")) {
    try {
        Invoke-RestMethod -Method Post -Uri $uri -ContentType 'application/json' -Body $body -TimeoutSec 10
    }
    catch {
        throw "Unable to set Magiline filtration through $uri. $($_.Exception.Message)"
    }
}
