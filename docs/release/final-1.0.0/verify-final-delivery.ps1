$ErrorActionPreference = 'Stop'
$root = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio'
$delivery = Join-Path $root 'DELIVERY'
$installer = Join-Path $delivery 'INSTALLER\CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe'
$report = Join-Path $delivery 'REPORTS\WEB3_COLLECTION_STUDIO_FINAL_FULL_CHAT_REPORT_AR.md'
$decision = Join-Path $delivery 'REPORTS\release-decision.md'
$tests = Join-Path $delivery 'REPORTS\final-tests.md'
$panelLaunch = Join-Path $delivery 'EVIDENCE\panel-web3-launch-result.json'
$loginImage = Join-Path $delivery 'EVIDENCE\installed-login-flat-prismatic-l-alpha-fixed.png'
$workstationImage = Join-Path $delivery 'EVIDENCE\installed-workstation-flat-prismatic-l-alpha-fixed.png'

$result = [pscustomobject]@{
    ReportInDelivery = Test-Path -LiteralPath $report
    DecisionVerifiedFinal = (Get-Content -Raw -LiteralPath $decision) -match 'Decision: VERIFIED FINAL 1\.0\.0'
    Regression7676Recorded = (Get-Content -Raw -LiteralPath $tests) -match '\| \*\*Total\*\* \| \*\*76\*\* \| \*\*0\*\* \| \*\*0\*\* \|'
    PanelLaunchDispatched = (Get-Content -Raw -LiteralPath $panelLaunch) -match 'LAUNCH DISPATCHED'
    InstalledLoginEvidence = Test-Path -LiteralPath $loginImage
    InstalledWorkstationEvidence = Test-Path -LiteralPath $workstationImage
    InstallerSha256 = (Get-FileHash -LiteralPath $installer -Algorithm SHA256).Hash
    InstallerSha256Matches = ((Get-FileHash -LiteralPath $installer -Algorithm SHA256).Hash -eq '67145663CCC7046A423E3F22D7BB65B7F1B2DE54B457B4BBDC105A370ABAF1A1')
}
$result | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath (Join-Path $root 'docs\release\final-1.0.0\final-delivery-verification.json') -Encoding utf8
if ($result.PSObject.Properties.Value -contains $false) { throw 'One or more Final delivery verification conditions failed.' }
