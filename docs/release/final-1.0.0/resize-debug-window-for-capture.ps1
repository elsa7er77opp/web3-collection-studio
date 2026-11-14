$ErrorActionPreference = 'Stop'
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativeWindowResize
{
    [DllImport("user32.dll")] public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
}
'@

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
[NativeWindowResize]::MoveWindow($process.MainWindowHandle, 18, 18, 1240, 790, $true) | Out-Null
[NativeWindowResize]::SetForegroundWindow($process.MainWindowHandle) | Out-Null
Start-Sleep -Milliseconds 500
