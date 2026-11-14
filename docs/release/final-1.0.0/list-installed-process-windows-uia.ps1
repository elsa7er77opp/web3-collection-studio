Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$all = [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($window in $all) {
    if ($window.Current.ProcessId -eq $process.Id) {
        [pscustomobject]@{ Name = $window.Current.Name; AutomationId = $window.Current.AutomationId; Handle = $window.Current.NativeWindowHandle; Type = $window.Current.ControlType.ProgrammaticName }
    }
}
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-process-windows-uia.json'
