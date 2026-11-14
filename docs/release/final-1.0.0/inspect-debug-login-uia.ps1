$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$root = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$items = $root.FindAll(
    [System.Windows.Automation.TreeScope]::Descendants,
    [System.Windows.Automation.Condition]::TrueCondition)

$result = $items | ForEach-Object {
    [PSCustomObject]@{
        ControlType = $_.Current.ControlType.ProgrammaticName
        Name = $_.Current.Name
        AutomationId = $_.Current.AutomationId
        IsEnabled = $_.Current.IsEnabled
    }
}

$result | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\debug-login-uia.json'
