param(
    [Parameter(Mandatory)]
    [SecureString]$QaPassword
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)

function Find-ByAutomationId([string]$automationId) {
    return $window.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::AutomationIdProperty, $automationId)))
}

$username = 'final.qa'
# UI Automation requires a plaintext string at the point of entry.
$password = [System.Net.NetworkCredential]::new('', $QaPassword).Password
try {
    foreach ($entry in @(@('UsernameBox', $username), @('PasswordBox', $password), @('PasswordConfirmationBox', $password))) {
        $element = Find-ByAutomationId $entry[0]
        if ($null -eq $element) { throw "QA setup control $($entry[0]) was not found." }
        $pattern = $element.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        ([System.Windows.Automation.ValuePattern]$pattern).SetValue($entry[1])
    }
} finally {
    Remove-Variable password, entry -ErrorAction SilentlyContinue
}

$button = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, 'CONTINUE LOCALLY')))
if ($null -eq $button) { throw 'Continue locally button was not found.' }
([System.Windows.Automation.InvokePattern]$button.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)).Invoke()
Start-Sleep -Seconds 2
$current = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
"PostSubmitTitle: $($current.MainWindowTitle)" | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-qa-setup-result.txt'
