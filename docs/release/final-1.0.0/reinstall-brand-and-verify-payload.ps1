$ErrorActionPreference = 'Stop'

$installer = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\installer\CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe'
$publishedDll = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\Web3CollectionStudio-Final-win-x64\Web3CollectionStudio.Desktop.dll'
$installedDll = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio\Web3CollectionStudio.Desktop.dll'
$log = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\brand-reinstall-inno.log'
$result = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\brand-reinstall-payload-verification.txt'

Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
$deadline = [DateTime]::UtcNow.AddSeconds(15)
while (Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue) {
    if ([DateTime]::UtcNow -gt $deadline) {
        throw 'Web3 Collection Studio did not release its process before reinstall.'
    }
    Start-Sleep -Milliseconds 250
}

$installerProcess = Start-Process -FilePath $installer -ArgumentList @('/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', "/LOG=$log") -Wait -PassThru
if ($installerProcess.ExitCode -ne 0) {
    throw "Installer returned exit code $($installerProcess.ExitCode)."
}

$publishedHash = (Get-FileHash -LiteralPath $publishedDll -Algorithm SHA256).Hash
$installedHash = (Get-FileHash -LiteralPath $installedDll -Algorithm SHA256).Hash
@(
    "PublishedSHA256: $publishedHash"
    "InstalledSHA256: $installedHash"
    "Matches: $($publishedHash -eq $installedHash)"
) | Set-Content -Encoding utf8 $result

if ($publishedHash -ne $installedHash) {
    throw 'Installed Desktop DLL does not match the Final published payload after a process-clean reinstall.'
}
