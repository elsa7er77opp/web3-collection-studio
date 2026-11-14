$payload = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\Web3CollectionStudio-Final-win-x64\Web3CollectionStudio.Infrastructure.dll'
$installed = 'C:\Users\ahmed\AppData\Local\Programs\CyberBank\Web3 Collection Studio\Web3CollectionStudio.Infrastructure.dll'
$payloadHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $payload).Hash
$installedHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $installed).Hash
@(
    "PayloadSha256: $payloadHash",
    "InstalledSha256: $installedHash",
    "Match: $($payloadHash -eq $installedHash)"
) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\infrastructure-dll-comparison.txt'
