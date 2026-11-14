$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$root = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$automationId = [System.Windows.Automation.AutomationElement]::AutomationIdProperty
$valuePattern = [System.Windows.Automation.ValuePattern]::Pattern
$invokePattern = [System.Windows.Automation.InvokePattern]::Pattern

function Find-ById([string]$id) {
    return $root.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        (New-Object System.Windows.Automation.PropertyCondition($automationId, $id)))
}

$usernameBox = Find-ById 'UsernameBox'
$passwordBox = Find-ById 'PasswordBox'
$confirmationBox = Find-ById 'PasswordConfirmationBox'
if ($null -in @($usernameBox, $passwordBox, $confirmationBox)) { throw 'Expected local setup fields were not available.' }

$username = "finalqa$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
$password = "NftQa!$([Guid]::NewGuid().ToString('N').Substring(0, 12))"
$usernameBox.GetCurrentPattern($valuePattern).SetValue($username)
$passwordBox.GetCurrentPattern($valuePattern).SetValue($password)
$confirmationBox.GetCurrentPattern($valuePattern).SetValue($password)

$continueButton = $root.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'CONTINUE LOCALLY')))
if ($null -eq $continueButton) { throw 'Expected local setup command was not available.' }
$continueButton.GetCurrentPattern($invokePattern).Invoke()
Start-Sleep -Seconds 2

Get-Process Web3CollectionStudio.Desktop | Select-Object Id, ProcessName, MainWindowTitle | Format-List
