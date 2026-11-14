param([Parameter(Mandatory = $true)][string]$LayerName)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$nameBox = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'LayerNameBox')))
if ($null -eq $nameBox) { throw 'Layer name control was not found.' }
([System.Windows.Automation.ValuePattern]$nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)).SetValue($LayerName)
$add = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Add layer')))
if ($null -eq $add) { throw 'Add layer button was not found.' }
([System.Windows.Automation.InvokePattern]$add.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
Start-Sleep -Seconds 1
"PostLayerTitle: $((Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1).MainWindowTitle)" | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-layer-add-result.txt'
