$ErrorActionPreference = 'Stop'
$installer = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\installer\CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe'
$item = Get-Item -LiteralPath $installer
$hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $installer).Hash
@(
    "Name: $($item.Name)",
    "Bytes: $($item.Length)",
    "SHA-256: $hash"
) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installer-sha256.txt'
