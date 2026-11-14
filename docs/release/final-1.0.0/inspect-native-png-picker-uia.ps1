Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$root = [System.Windows.Automation.AutomationElement]::RootElement
$picker = $root.FindFirst(
    [System.Windows.Automation.TreeScope]::Children,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Select local PNG trait')))
if ($null -eq $picker) { throw 'Native PNG picker was not found.' }
$all = $picker.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($element in $all) {
    $type = $element.Current.ControlType.ProgrammaticName
    if ($type -in @('ControlType.Edit', 'ControlType.Button', 'ControlType.ComboBox')) {
        [pscustomobject]@{ Name = $element.Current.Name; AutomationId = $element.Current.AutomationId; Type = $type; Enabled = $element.Current.IsEnabled }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\native-png-picker-uia.json'
