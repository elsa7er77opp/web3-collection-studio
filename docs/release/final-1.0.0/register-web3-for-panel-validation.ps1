$ErrorActionPreference = 'Stop'

$installRoot = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio'
$manifestPath = Join-Path $installRoot 'cyberbank-app.json'
$registryDirectory = Join-Path $env:LOCALAPPDATA 'CyberBank\Panel'
$registryPath = Join-Path $registryDirectory 'registry.json'
$evidenceRoot = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0'

$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$executablePath = Join-Path $installRoot $manifest.executablePath
$iconPath = Join-Path $installRoot $manifest.iconPath
if (-not (Test-Path -LiteralPath $executablePath) -or -not (Test-Path -LiteralPath $iconPath)) {
    throw 'The installed NFT Studio manifest does not resolve to an available executable and icon.'
}

New-Item -ItemType Directory -Force -Path $registryDirectory | Out-Null
if (Test-Path -LiteralPath $registryPath) {
    Copy-Item -LiteralPath $registryPath -Destination (Join-Path $registryDirectory 'registry.before-web3-panel-validation.json.bak') -Force
    $registrations = @(Get-Content -Raw -LiteralPath $registryPath | ConvertFrom-Json)
}
else {
    $registrations = @()
}

$registration = [pscustomobject]@{
    AppId = [string]$manifest.appId
    DisplayName = [string]$manifest.displayName
    Version = [string]$manifest.version
    InstallRoot = $installRoot.TrimEnd('\')
    ExecutablePath = $executablePath
    IconPath = $iconPath
    Capabilities = @($manifest.capabilities)
}

$updatedRegistrations = @($registrations | Where-Object { $_.AppId -ne $registration.AppId }) + @($registration)
$serializedRegistrations = ConvertTo-Json -InputObject @($updatedRegistrations) -Depth 6
if (-not $serializedRegistrations.TrimStart().StartsWith('[')) {
    throw 'Panel registry must serialize as a JSON array.'
}
$serializedRegistrations | Set-Content -LiteralPath $registryPath -Encoding utf8

[pscustomobject]@{
    AppId = $registration.AppId
    DisplayName = $registration.DisplayName
    Version = $registration.Version
    ExecutableExists = Test-Path -LiteralPath $registration.ExecutablePath
    IconExists = Test-Path -LiteralPath $registration.IconPath
    RegistryPath = $registryPath
    RegistrationMethod = 'Manifest-derived registry entry for Panel host launch validation; pre-existing registry backed up.'
} | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $evidenceRoot 'panel-registration-result.json') -Encoding utf8
