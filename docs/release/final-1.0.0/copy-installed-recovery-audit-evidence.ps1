$audit = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final PFP QA-9cfd9fa9e3d84178be43a896ae4842c8\.collection-studio\project-events.jsonl'
$evidence = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-recovery-audit.jsonl'
Copy-Item -LiteralPath $audit -Destination $evidence -Force
