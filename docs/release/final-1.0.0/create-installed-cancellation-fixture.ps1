Add-Type -AssemblyName System.Drawing
$path = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\fixtures\cancellation-indigo-4096.png'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $path) | Out-Null
$bitmap = New-Object System.Drawing.Bitmap(4096, 4096)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::FromArgb(255, 56, 45, 145))
$graphics.Dispose()
$bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
$bitmap.Dispose()
Get-Item -LiteralPath $path | Select-Object FullName, Length | Format-Table -AutoSize | Out-String |
    Set-Content -Encoding utf8 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\cancellation-fixture.txt'
