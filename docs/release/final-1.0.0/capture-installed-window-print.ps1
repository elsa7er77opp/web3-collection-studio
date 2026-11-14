Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativePrintWindowCapture {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint flags);
}
'@

$process = Get-Process Web3CollectionStudio.Desktop | Select-Object -First 1
if ($null -eq $process -or $process.MainWindowHandle -eq [IntPtr]::Zero) { throw 'Installed Workstation window was not available.' }

$rect = New-Object NativePrintWindowCapture+RECT
if (-not [NativePrintWindowCapture]::GetWindowRect($process.MainWindowHandle, [ref]$rect)) { throw 'Unable to read installed window bounds.' }

$width = $rect.Right - $rect.Left
$height = $rect.Bottom - $rect.Top
$bitmap = New-Object System.Drawing.Bitmap($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$hdc = $graphics.GetHdc()
try {
    if (-not [NativePrintWindowCapture]::PrintWindow($process.MainWindowHandle, $hdc, 2)) { throw 'PrintWindow did not produce a capture.' }
}
finally {
    $graphics.ReleaseHdc($hdc)
}

$bitmap.Save('C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-workstation-flat-prismatic-l-alpha-fixed.png', [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()
