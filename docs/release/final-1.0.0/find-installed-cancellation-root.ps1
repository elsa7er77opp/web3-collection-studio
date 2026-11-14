$collections = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections'
$root = Get-ChildItem -LiteralPath $collections -Directory |
    Where-Object { $_.Name -like 'Final Cancellation QA-*' } |
    Sort-Object LastWriteTimeUtc -Descending |
    Select-Object -First 1
if ($null -eq $root) { throw 'Final Cancellation QA root was not found.' }
$root.FullName | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-cancellation-root.txt'
