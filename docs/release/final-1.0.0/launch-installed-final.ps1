$ErrorActionPreference = 'Stop'
$exe = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio\Web3CollectionStudio.Desktop.exe'
Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process -FilePath $exe
Start-Sleep -Seconds 3
$process = Get-Process Web3CollectionStudio.Desktop -ErrorAction Stop | Select-Object -First 1
@(
    "ProcessId: $($process.Id)",
    "Title: $($process.MainWindowTitle)",
    "Path: $($process.Path)"
) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-launch-smoke.txt'
