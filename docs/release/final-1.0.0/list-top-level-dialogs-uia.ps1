Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$windows = [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($window in $windows) {
    if ($window.Current.ControlType.ProgrammaticName -eq 'ControlType.Window') {
        $edits = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit))).Count
        if ($edits -gt 0) {
            [pscustomobject]@{ Name = $window.Current.Name; ProcessId = $window.Current.ProcessId; Handle = $window.Current.NativeWindowHandle; EditCount = $edits }
        }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\top-level-dialogs-uia.json'
