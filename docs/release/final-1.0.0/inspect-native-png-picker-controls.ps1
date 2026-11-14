Add-Type @'
using System;
using System.Text;
using System.Runtime.InteropServices;
public static class NativePngPickerInspector {
    public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc callback, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetClassName(IntPtr hWnd, StringBuilder text, int maxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);
    [DllImport("user32.dll")] public static extern int GetDlgCtrlID(IntPtr hWnd);
}
'@

$dialog = [NativePngPickerInspector]::FindWindow('#32770', 'Select local PNG trait')
if ($dialog -eq [IntPtr]::Zero) { throw 'Native PNG picker handle was not found.' }
$rows = New-Object System.Collections.Generic.List[object]
$callback = [NativePngPickerInspector+EnumChildProc]{
    param([IntPtr]$handle, [IntPtr]$lParam)
    $class = New-Object System.Text.StringBuilder 256
    $text = New-Object System.Text.StringBuilder 2048
    [NativePngPickerInspector]::GetClassName($handle, $class, $class.Capacity) | Out-Null
    [NativePngPickerInspector]::GetWindowText($handle, $text, $text.Capacity) | Out-Null
    $rows.Add([pscustomobject]@{ Handle = $handle.ToInt64(); Id = [NativePngPickerInspector]::GetDlgCtrlID($handle); Class = $class.ToString(); Text = $text.ToString() }) | Out-Null
    return $true
}
[NativePngPickerInspector]::EnumChildWindows($dialog, $callback, [IntPtr]::Zero) | Out-Null
$rows | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\native-png-picker-controls.json'
