Add-Type -AssemblyName System.Windows.Forms
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class NativePickerInput {
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extraInfo);
}
'@

$fixture = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\fixtures\acceptance-indigo.png'
Set-Clipboard -Value $fixture
[NativePickerInput]::SetCursorPos(430, 548) | Out-Null
[NativePickerInput]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
[NativePickerInput]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 150
[System.Windows.Forms.SendKeys]::SendWait('^a')
[System.Windows.Forms.SendKeys]::SendWait('^v')
Start-Sleep -Milliseconds 300
[NativePickerInput]::SetCursorPos(752, 584) | Out-Null
[NativePickerInput]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
[NativePickerInput]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
