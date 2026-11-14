Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$all = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($element in $all) {
    $text = $element.Current.Name
    if ($text -and $text -match 'BACKUP|Backup|RECOVER|Recover|OPEN|Open|LOCAL PROJECT|Local project|Final PFP') {
        [pscustomobject]@{ Type = $element.Current.ControlType.ProgrammaticName; Text = $text }
    }
}
$rows | Sort-Object Text -Unique | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-recovery-feedback.json'
