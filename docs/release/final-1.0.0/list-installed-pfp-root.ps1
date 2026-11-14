$root = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final PFP QA-9cfd9fa9e3d84178be43a896ae4842c8'
Get-ChildItem -Force -Recurse -LiteralPath $root |
    Select-Object FullName, Length, LastWriteTime |
    Format-Table -AutoSize |
    Out-String -Width 240 |
    Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-pfp-root.txt'
