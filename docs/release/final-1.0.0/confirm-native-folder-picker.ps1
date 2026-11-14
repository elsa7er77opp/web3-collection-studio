Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativeFolderPickerConfirm {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
    [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
'@
$dialog = [NativeFolderPickerConfirm]::FindWindow('#32770', 'Open local collection project')
if ($dialog -eq [IntPtr]::Zero) { throw 'Native folder picker handle was not found.' }
$select = [NativeFolderPickerConfirm]::GetDlgItem($dialog, 1)
if ($select -eq [IntPtr]::Zero) { throw 'Select Folder button was not found.' }
[NativeFolderPickerConfirm]::SendMessage($select, 0x00F5, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
