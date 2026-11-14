$root = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final PFP QA-9cfd9fa9e3d84178be43a896ae4842c8'
$state = Join-Path $root '.collection-studio'
$primary = Join-Path $state 'project.json'
$backup = Join-Path $state 'project.json.bak'
$evidence = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0'
Copy-Item -LiteralPath $primary -Destination (Join-Path $evidence 'installed-recovery-primary.json') -Force
Copy-Item -LiteralPath $backup -Destination (Join-Path $evidence 'installed-recovery-backup.json') -Force
Get-Item -LiteralPath $primary, $backup |
    Select-Object FullName, Length, LastWriteTime |
    Format-Table -AutoSize |
    Out-String -Width 240 |
    Set-Content -Encoding utf8 (Join-Path $evidence 'installed-recovery-manifest-metadata.txt')
