$ErrorActionPreference = 'Stop'
$stateDirectory = Join-Path $env:LOCALAPPDATA 'CyberBank\Web3CollectionStudio'
$account = Join-Path $stateDirectory 'account.json'
Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force

if (Test-Path -LiteralPath $account) {
    $backup = Join-Path $stateDirectory ("account.json.qa-isolated-" + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.bak')
    Move-Item -LiteralPath $account -Destination $backup
    @(
        "Action: account.json moved to QA backup; no collection project or asset path was accessed.",
        "StateDirectory: $stateDirectory",
        "BackupFile: $backup"
    ) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\qa-account-isolation.txt'
}
else {
    'Action: no existing account.json; no account state changed.' | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\qa-account-isolation.txt'
}
