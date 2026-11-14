$root = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final Cancellation QA-d856ad77b1084ba290f04acd32723d3d'
$source = Join-Path $root 'render-events.jsonl'
$destination = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-cancellation-audit.jsonl'
Copy-Item -LiteralPath $source -Destination $destination -Force
