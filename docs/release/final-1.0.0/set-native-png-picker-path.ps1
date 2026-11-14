param([Parameter(Mandatory = $true)][string]$Fixture)

Add-Type @'
using System;
using System.Text;
using System.Runtime.InteropServices;
public static class NativePngPicker {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowName);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);
}
'@

$dialog = [NativePngPicker]::FindWindow('#32770', 'Select local PNG trait')
if ($dialog -eq [IntPtr]::Zero) { throw 'Native PNG picker handle was not found.' }
$comboEx = [NativePngPicker]::GetDlgItem($dialog, 1148)
$combo = [NativePngPicker]::FindWindowEx($comboEx, [IntPtr]::Zero, 'ComboBox', $null)
$fileName = [NativePngPicker]::FindWindowEx($combo, [IntPtr]::Zero, 'Edit', $null)
if ($fileName -eq [IntPtr]::Zero) { throw 'Native File name Edit control was not found.' }
[NativePngPicker]::SendMessage($fileName, 0x000C, [IntPtr]::Zero, $fixture) | Out-Null
$text = New-Object System.Text.StringBuilder 2048
[NativePngPicker]::GetWindowText($fileName, $text, $text.Capacity) | Out-Null
"FileNameValue: $($text.ToString())" | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\native-png-picker-path.txt'
