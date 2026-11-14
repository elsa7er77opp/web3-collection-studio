$ErrorActionPreference = 'Stop'
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativeInstalledWindowRestore {
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int command);
    [DllImport("user32.dll")] public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
}
'@

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
if ($null -eq $process -or $process.MainWindowHandle -eq [IntPtr]::Zero) { throw 'Installed application window was not available.' }

[NativeInstalledWindowRestore]::ShowWindow($process.MainWindowHandle, 9) | Out-Null
[NativeInstalledWindowRestore]::MoveWindow($process.MainWindowHandle, 20, 20, 1240, 790, $true) | Out-Null
[NativeInstalledWindowRestore]::SetForegroundWindow($process.MainWindowHandle) | Out-Null
Start-Sleep -Milliseconds 750
