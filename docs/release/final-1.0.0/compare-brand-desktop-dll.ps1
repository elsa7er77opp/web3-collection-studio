$ErrorActionPreference = 'Stop'

$publish = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\Web3CollectionStudio-Final-win-x64\Web3CollectionStudio.Desktop.dll'
$installed = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio\Web3CollectionStudio.Desktop.dll'

function Describe-Dll([string] $label, [string] $path) {
    $item = Get-Item -LiteralPath $path
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($path)
    [pscustomobject]@{
        Label = $label
        Path = $path
        Bytes = $item.Length
        SHA256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
        FileVersion = $version.FileVersion
        ProductVersion = $version.ProductVersion
    }
}

@(
    Describe-Dll 'Published payload' $publish
    Describe-Dll 'Installed LocalAppData' $installed
) | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\brand-desktop-dll-comparison.json'
