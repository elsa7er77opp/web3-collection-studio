param([Parameter(Mandatory = $true)][string]$Root)

Add-Type @'
using System;
using System.Text;
using System.Runtime.InteropServices;
public static class NativeFolderPickerInput {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);
}
'@
$dialog = [NativeFolderPickerInput]::FindWindow('#32770', 'Open local collection project')
if ($dialog -eq [IntPtr]::Zero) { throw 'Native folder picker handle was not found.' }
$folder = [NativeFolderPickerInput]::GetDlgItem($dialog, 1152)
if ($folder -eq [IntPtr]::Zero) { throw 'Native Folder edit control was not found.' }
[NativeFolderPickerInput]::SendMessage($folder, 0x000C, [IntPtr]::Zero, $root) | Out-Null
$text = New-Object System.Text.StringBuilder 2048
[NativeFolderPickerInput]::GetWindowText($folder, $text, $text.Capacity) | Out-Null
"FolderValue: $($text.ToString())" | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\native-folder-picker-path.txt'
