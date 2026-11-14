Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$root = [System.Windows.Automation.AutomationElement]::RootElement
$window = $root.FindFirst(
    [System.Windows.Automation.TreeScope]::Children,
    (New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $process.Id)),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'New local collection')))))
if ($null -eq $window) { throw 'New local collection dialog was not found.' }
$all = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($element in $all) {
    $type = $element.Current.ControlType.ProgrammaticName
    if ($type -in @('ControlType.Edit', 'ControlType.Button', 'ControlType.RadioButton', 'ControlType.Text')) {
        [pscustomobject]@{ Name = $element.Current.Name; AutomationId = $element.Current.AutomationId; Type = $type; Enabled = $element.Current.IsEnabled }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-new-collection-uia.json'
