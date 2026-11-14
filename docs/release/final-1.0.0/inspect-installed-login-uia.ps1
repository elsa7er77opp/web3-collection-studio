Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$all = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($element in $all) {
    $type = $element.Current.ControlType.ProgrammaticName
    if ($type -in @('ControlType.Edit', 'ControlType.Button', 'ControlType.Text')) {
        [pscustomobject]@{ Name = $element.Current.Name; AutomationId = $element.Current.AutomationId; Type = $type; Enabled = $element.Current.IsEnabled }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-login-uia.json'
