Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativePngPickerConfirm {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
    [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
'@
$dialog = [NativePngPickerConfirm]::FindWindow('#32770', 'Select local PNG trait')
if ($dialog -eq [IntPtr]::Zero) { throw 'Native PNG picker handle was not found.' }
$open = [NativePngPickerConfirm]::GetDlgItem($dialog, 1)
if ($open -eq [IntPtr]::Zero) { throw 'Native Open button was not found.' }
[NativePngPickerConfirm]::SendMessage($open, 0x00F5, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
