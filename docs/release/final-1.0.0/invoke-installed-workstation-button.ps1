param([Parameter(Mandatory = $true)][string]$ButtonName)
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$button = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $ButtonName)))
if ($null -eq $button) { throw "Button '$ButtonName' was not found." }
if (-not $button.Current.IsEnabled) { throw "Button '$ButtonName' is disabled." }
([System.Windows.Automation.InvokePattern]$button.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
