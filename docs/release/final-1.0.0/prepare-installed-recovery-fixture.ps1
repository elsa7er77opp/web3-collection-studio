$root = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final PFP QA-9cfd9fa9e3d84178be43a896ae4842c8'
$manifest = Join-Path $root '.collection-studio\project.json'
$backup = Join-Path $root '.collection-studio\project.json.bak'
Copy-Item -LiteralPath $manifest -Destination $backup -Force
Set-Content -LiteralPath $manifest -Value '{ invalid-manifest-for-final-acceptance' -Encoding utf8
@(
    "FixtureRoot: $root",
    "Primary: corrupt intentionally for QA recovery",
    "Backup: copied from valid current PFP QA manifest"
) | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-recovery-fixture.txt'
