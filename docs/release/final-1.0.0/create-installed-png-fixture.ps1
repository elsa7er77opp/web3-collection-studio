Add-Type -AssemblyName System.Drawing
$path = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\fixtures\acceptance-indigo.png'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $path) | Out-Null
$bitmap = New-Object System.Drawing.Bitmap(128, 128)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::FromArgb(255, 79, 70, 229))
$graphics.Dispose()
$bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
$bitmap.Dispose()
