param([Parameter(Mandatory = $true)][string]$ProjectRoot)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

function Find-Button([System.Windows.Automation.AutomationElement]$window, [string]$name) {
    return $window.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $name)))
}

$outputs = Join-Path $ProjectRoot 'outputs'
$before = @(Get-ChildItem -LiteralPath $outputs -Filter '*.png' -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name)
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$render = Find-Button $window 'Render one local PNG'
if ($null -eq $render -or -not $render.Current.IsEnabled) { throw 'Render one local PNG was not available.' }
([System.Windows.Automation.InvokePattern]$render.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()

$cancelled = $false
for ($attempt = 0; $attempt -lt 1000; $attempt++) {
    $cancel = Find-Button $window 'Cancel local render'
    if ($null -ne $cancel -and $cancel.Current.IsEnabled) {
        ([System.Windows.Automation.InvokePattern]$cancel.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
        $cancelled = $true
        break
    }
    Start-Sleep -Milliseconds 2
}
Start-Sleep -Milliseconds 1500
$after = @(Get-ChildItem -LiteralPath $outputs -Filter '*.png' -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name)
[pscustomobject]@{
    CancelInvoked = $cancelled
    NewOutputFiles = @($after | Where-Object { $_ -notin $before })
} | ConvertTo-Json -Depth 4 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-cancellation-result.json'
