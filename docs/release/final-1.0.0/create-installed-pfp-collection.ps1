param([Parameter(Mandatory = $true)][string]$CollectionName)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$nameBox = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'CollectionNameBox')))
if ($null -eq $nameBox) { throw 'Collection name control was not found.' }
([System.Windows.Automation.ValuePattern]$nameBox.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)).SetValue($CollectionName)
$create = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Create locally')))
if ($null -eq $create) { throw 'Create locally button was not found.' }
([System.Windows.Automation.InvokePattern]$create.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
Start-Sleep -Seconds 2
$current = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
"PostCreateTitle: $($current.MainWindowTitle)" | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-pfp-create-result.txt'
