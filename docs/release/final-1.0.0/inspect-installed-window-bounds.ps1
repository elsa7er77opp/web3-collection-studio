Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativeBounds {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
}
'@

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
$window = [System.Windows.Automation.AutomationElement]::FromHandle($process.MainWindowHandle)
$uiBounds = $window.Current.BoundingRectangle
$nativeBounds = New-Object NativeBounds+RECT
$nativeRead = [NativeBounds]::GetWindowRect($process.MainWindowHandle, [ref]$nativeBounds)

[pscustomobject]@{
    ProcessId = $process.Id
    Handle = $process.MainWindowHandle.ToInt64()
    Title = $process.MainWindowTitle
    UiLeft = $uiBounds.Left
    UiTop = $uiBounds.Top
    UiWidth = $uiBounds.Width
    UiHeight = $uiBounds.Height
    NativeRead = $nativeRead
    NativeLeft = $nativeBounds.Left
    NativeTop = $nativeBounds.Top
    NativeWidth = $nativeBounds.Right - $nativeBounds.Left
    NativeHeight = $nativeBounds.Bottom - $nativeBounds.Top
} | ConvertTo-Json -Depth 3 | Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-workstation-bounds.json'
