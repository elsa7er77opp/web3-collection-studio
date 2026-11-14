$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativeSplashCapture {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint flags);
}
'@

$exe = Join-Path $env:LOCALAPPDATA 'Programs\CyberBank\Web3 Collection Studio\Web3CollectionStudio.Desktop.exe'
Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process -FilePath $exe

$deadline = [DateTime]::UtcNow.AddSeconds(10)
$process = $null
while ([DateTime]::UtcNow -lt $deadline) {
    $candidate = Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -ne $candidate -and $candidate.MainWindowHandle -ne [IntPtr]::Zero -and $candidate.MainWindowTitle -eq 'CyberBank NFT Studio') {
        $process = $candidate
        break
    }
    Start-Sleep -Milliseconds 30
}
if ($null -eq $process) { throw 'Installed Splash window did not become available before Login replaced it.' }

$rect = New-Object NativeSplashCapture+RECT
if (-not [NativeSplashCapture]::GetWindowRect($process.MainWindowHandle, [ref]$rect)) { throw 'Unable to read Splash bounds.' }
$width = $rect.Right - $rect.Left
$height = $rect.Bottom - $rect.Top
if ($width -lt 400 -or $height -lt 300) { throw "Unexpected Splash geometry ${width}x${height}." }

$bitmap = New-Object System.Drawing.Bitmap($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$hdc = $graphics.GetHdc()
try {
    if (-not [NativeSplashCapture]::PrintWindow($process.MainWindowHandle, $hdc, 2)) { throw 'PrintWindow did not produce a Splash capture.' }
}
finally {
    $graphics.ReleaseHdc($hdc)
}

$output = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-splash-flat-prismatic-l-alpha-fixed.png'
$bitmap.Save($output, [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()
