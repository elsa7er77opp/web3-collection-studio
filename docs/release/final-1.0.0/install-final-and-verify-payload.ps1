$ErrorActionPreference = 'Stop'
$installer = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\installer\CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe'
$installRoot = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio'
$exe = Join-Path $installRoot 'Web3CollectionStudio.Desktop.exe'
$manifest = Join-Path $installRoot 'cyberbank-app.json'
$icon = Join-Path $installRoot 'Assets\Brand\collection-studio-app-icon.ico'

Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
$process = Start-Process -FilePath $installer -ArgumentList '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART' -Wait -PassThru
if ($process.ExitCode -ne 0) { throw "Final installer failed with exit code $($process.ExitCode)." }
if (-not (Test-Path -LiteralPath $exe)) { throw 'Installed Final executable was not found.' }
if (-not (Test-Path -LiteralPath $manifest)) { throw 'Installed Final Panel manifest was not found.' }
if (-not (Test-Path -LiteralPath $icon)) { throw 'Installed Final icon was not found.' }

@(
    "InstallRoot: $installRoot",
    "Executable: $exe",
    "Manifest: $manifest",
    "Icon: $icon",
    "PanelVersion: $((Get-Content -Raw -LiteralPath $manifest | ConvertFrom-Json).version)",
    "PanelExecutablePath: $((Get-Content -Raw -LiteralPath $manifest | ConvertFrom-Json).executablePath)",
    "PanelIconPath: $((Get-Content -Raw -LiteralPath $manifest | ConvertFrom-Json).iconPath)"
) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-payload-verification.txt'
