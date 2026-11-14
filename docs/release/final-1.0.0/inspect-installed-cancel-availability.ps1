Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$cancel = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'Cancel local render')))
$texts = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) |
    ForEach-Object { $_.Current.Name } |
    Where-Object { $_ -match 'No active local render|Cancellation available' } |
    Sort-Object -Unique
[pscustomobject]@{
    CancelButtonFound = $null -ne $cancel
    CancelButtonEnabled = if ($null -eq $cancel) { $null } else { $cancel.Current.IsEnabled }
    AvailabilityTexts = @($texts)
} | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-cancel-availability.json'
