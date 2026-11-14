Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$process = Get-Process CyberBankPanel.Desktop | Select-Object -First 1
if ($null -eq $process -or $process.MainWindowHandle -eq [IntPtr]::Zero) { throw 'CyberBank Panel window was not available.' }

$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$elements = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($element in $elements) {
    $type = $element.Current.ControlType.ProgrammaticName
    if ($type -in @('ControlType.Button', 'ControlType.ListItem', 'ControlType.Text', 'ControlType.Edit')) {
        [pscustomobject]@{
            Name = $element.Current.Name
            AutomationId = $element.Current.AutomationId
            Type = $type
            Enabled = $element.Current.IsEnabled
        }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\panel-uia-inventory.json'
