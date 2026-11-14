$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 350

$panelProcess = Get-Process CyberBankPanel.Desktop | Select-Object -First 1
if ($null -eq $panelProcess -or $panelProcess.MainWindowHandle -eq [IntPtr]::Zero) { throw 'CyberBank Panel host window was not available.' }
$panelWindow = [System.Windows.Automation.AutomationElement]::FromHandle($panelProcess.MainWindowHandle)
$buttonCondition = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Launch selected product')
$launchButton = $panelWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $buttonCondition)
if ($null -eq $launchButton) { throw 'Panel Launch selected product button was not found.' }
($launchButton.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
Start-Sleep -Seconds 3

$statusTexts = $panelWindow.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) |
    ForEach-Object { $_.Current.Name } |
    Where-Object { $_ -match 'LAUNCH DISPATCHED|LAUNCH BLOCKED' }
$product = Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Select-Object -First 1

[pscustomobject]@{
    PanelProcessId = $panelProcess.Id
    InvokedButton = 'Launch selected product'
    PanelStatus = @($statusTexts) -join ' | '
    ProductProcessStarted = $null -ne $product
    ProductProcessId = if ($null -ne $product) { $product.Id } else { $null }
    ProductMainWindowTitle = if ($null -ne $product) { $product.MainWindowTitle } else { $null }
    ProductResponding = if ($null -ne $product) { $product.Responding } else { $false }
} | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\panel-web3-launch-result.json'
