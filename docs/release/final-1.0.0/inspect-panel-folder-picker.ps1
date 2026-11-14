$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms

$panel = Get-Process CyberBankPanel.Desktop | Select-Object -First 1
$panelWindow = [System.Windows.Automation.AutomationElement]::FromHandle($panel.MainWindowHandle)
$buttonCondition = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Register local app')
$registerButton = $panelWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $buttonCondition)
if ($null -eq $registerButton) { throw 'Register local app button was not found.' }
($registerButton.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
Start-Sleep -Milliseconds 750

$children = [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
$rows = foreach ($child in $children) {
    [pscustomobject]@{
        Name = $child.Current.Name
        AutomationId = $child.Current.AutomationId
        ProcessId = $child.Current.ProcessId
        Handle = $child.Current.NativeWindowHandle
        Type = $child.Current.ControlType.ProgrammaticName
    }
}
@($rows) | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\panel-folder-picker-uia.json'

[System.Windows.Forms.SendKeys]::SendWait('{ESC}')
